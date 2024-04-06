using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess;

public class Board : ICloneable
{
    public Board(bool clearBoard = false) 
    {
        if (!clearBoard)
        {
            SetPiecesToDefault();
            UpdateSituations();
        }
    }

    public Board Move(string algebraic, PieceColor color)
    {
        var move = new Move(algebraic, this, color);
        var applyBoard = LegalMoves.ApplyMove(move.PreviousPos, move.CurrentPos, this);
        applyBoard.Moves.Add(move);
        applyBoard.UpdateSituations();
        return applyBoard;
    }

    public Piece?[] Pieces = new Piece[64];

    public List<Move> Moves = new List<Move>();

    public record Situation
    {
        public bool IsChecked { get; set; } = false;
        public bool IsCheckMated { get; set; } = false;
        public bool IsKingMoved { get; set; } = false;
        public bool IsARookMoved { get; set; } = false;
        public bool IsHRookMoved { get; set; } = false;
    }
    
    public Dictionary<PieceColor , Situation> Situations 
    { 
        get 
        {
            return new Dictionary<PieceColor, Situation>()
            {
                {PieceColor.White , WhiteSituation },
                {PieceColor.Black , BlackSituation }
            };
        } 
    }
    
    private Situation WhiteSituation { get; set; } = new Situation();
    
    private Situation BlackSituation { get; set; } = new Situation();
    
    private void SetPiecesToDefault()
    {
        var pos = new Position();
        var typeDict = new Dictionary<int, PieceType>
        {
            {pos["a1"].Index,PieceType.Rook}, {pos["b1"].Index,PieceType.Knight},
            {pos["c1"].Index,PieceType.Bishop}, {pos["d1"].Index,PieceType.Queen},
            {pos["e1"].Index,PieceType.King}, {pos["f1"].Index,PieceType.Bishop},
            {pos["g1"].Index,PieceType.Knight}, {pos["h1"].Index,PieceType.Rook},
            {pos["a2"].Index,PieceType.Pawn}, {pos["b2"].Index,PieceType.Pawn},
            {pos["c2"].Index,PieceType.Pawn}, {pos["d2"].Index,PieceType.Pawn},
            {pos["e2"].Index,PieceType.Pawn}, {pos["f2"].Index,PieceType.Pawn},
            {pos["g2"].Index,PieceType.Pawn}, {pos["h2"].Index,PieceType.Pawn},
            {pos["a7"].Index,PieceType.Pawn}, {pos["b7"].Index,PieceType.Pawn},
            {pos["c7"].Index,PieceType.Pawn}, {pos["d7"].Index,PieceType.Pawn},
            {pos["e7"].Index,PieceType.Pawn}, {pos["f7"].Index,PieceType.Pawn},
            {pos["g7"].Index,PieceType.Pawn}, {pos["h7"].Index,PieceType.Pawn},
            {pos["a8"].Index,PieceType.Rook}, {pos["b8"].Index,PieceType.Knight},
            {pos["c8"].Index,PieceType.Bishop}, {pos["d8"].Index,PieceType.Queen},
            {pos["e8"].Index,PieceType.King}, {pos["f8"].Index,PieceType.Bishop},
            {pos["g8"].Index,PieceType.Knight}, {pos["h8"].Index,PieceType.Rook}
        };
        var colorDict = new Dictionary<int, PieceColor>
        {
            { pos["a1"].Index, PieceColor.White }, { pos["b1"].Index, PieceColor.White },
            { pos["c1"].Index, PieceColor.White }, { pos["d1"].Index, PieceColor.White },
            { pos["e1"].Index, PieceColor.White }, { pos["f1"].Index, PieceColor.White },
            { pos["g1"].Index, PieceColor.White }, { pos["h1"].Index, PieceColor.White },
            { pos["a2"].Index, PieceColor.White }, { pos["b2"].Index, PieceColor.White },
            { pos["c2"].Index, PieceColor.White }, { pos["d2"].Index, PieceColor.White },
            { pos["e2"].Index, PieceColor.White }, { pos["f2"].Index, PieceColor.White },
            { pos["g2"].Index, PieceColor.White }, { pos["h2"].Index, PieceColor.White },
            { pos["a7"].Index, PieceColor.Black }, { pos["b7"].Index, PieceColor.Black },
            { pos["c7"].Index, PieceColor.Black }, { pos["d7"].Index, PieceColor.Black },
            { pos["e7"].Index, PieceColor.Black }, { pos["f7"].Index, PieceColor.Black },
            { pos["g7"].Index, PieceColor.Black }, { pos["h7"].Index, PieceColor.Black },
            { pos["a8"].Index, PieceColor.Black }, { pos["b8"].Index, PieceColor.Black },
            { pos["c8"].Index, PieceColor.Black }, { pos["d8"].Index, PieceColor.Black },
            { pos["e8"].Index, PieceColor.Black }, { pos["f8"].Index, PieceColor.Black },
            { pos["g8"].Index, PieceColor.Black }, { pos["h8"].Index, PieceColor.Black }
        };
        for (var i = 0; i < 64; i++)
        {
            if (colorDict.TryGetValue(i, out var value))
            {
                Pieces[i] = new Piece { Type = typeDict[i], Color = value };
            }
        }
    }
    
    public void UpdateSituations()
    {
        if (Moves.Count == 0) return;
        if (Moves.Last().Piece.Color == PieceColor.White)
        {
            WhiteSituation.IsKingMoved = Moves.Any(x => x.Piece.Type == PieceType.King && x.Piece.Color == PieceColor.White);
            WhiteSituation.IsARookMoved = Moves.Any(x => x.Piece.Type == PieceType.Rook && x.Piece.Color == PieceColor.White && x.PreviousPos.Name == "a1");
            WhiteSituation.IsHRookMoved = Moves.Any(x => x.Piece.Type == PieceType.Rook && x.Piece.Color == PieceColor.White && x.PreviousPos.Name == "h1");
            BlackSituation.IsChecked = Moves.Last().Check;
            BlackSituation.IsCheckMated = Moves.Last().CheckMate;
            WhiteSituation.IsChecked = false;
            return;
        }
        
        BlackSituation.IsKingMoved = Moves.Any(x => x.Piece.Type == PieceType.King && x.Piece.Color == PieceColor.Black);
        BlackSituation.IsARookMoved = Moves.Any(x => x.Piece.Type == PieceType.Rook && x.Piece.Color == PieceColor.Black && x.PreviousPos.Name == "a8");
        BlackSituation.IsHRookMoved = Moves.Any(x => x.Piece.Type == PieceType.Rook && x.Piece.Color == PieceColor.Black && x.PreviousPos.Name == "h8");
        WhiteSituation.IsChecked = Moves.Last().Check;
        WhiteSituation.IsCheckMated = Moves.Last().CheckMate;
        BlackSituation.IsChecked = false;
    }
    
    public object Clone()
    {
        var board = new Board(true);
        for(int i = 0; i < 64; i++)
        {
            if (Pieces[i] is null) continue;
            board.Pieces[i] = (Piece?)Pieces[i].Clone(); 
        }
        board.Moves = new List<Move>(Moves);
        board.WhiteSituation = new Situation
        {
            IsARookMoved = WhiteSituation.IsARookMoved,
            IsChecked = WhiteSituation.IsChecked,
            IsCheckMated = WhiteSituation.IsCheckMated,
            IsHRookMoved = WhiteSituation.IsHRookMoved,
            IsKingMoved = WhiteSituation.IsKingMoved
        };
        board.BlackSituation = new Situation
        {
            IsARookMoved = BlackSituation.IsARookMoved,
            IsChecked = BlackSituation.IsChecked,
            IsCheckMated = BlackSituation.IsCheckMated,
            IsHRookMoved = BlackSituation.IsHRookMoved,
            IsKingMoved = BlackSituation.IsKingMoved
        };
        return board;
    }
    
}


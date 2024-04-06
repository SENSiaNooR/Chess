using System.Diagnostics;
using System.Drawing;

namespace Chess;

public class Move : ICloneable
{
    public Move(){}

    public Move(string algebraic, Board board, PieceColor color)
    {
        var move = AlgebraicConvert.AlgebraicToMove(algebraic, board, color);
        Piece = move.Piece;
        PreviousPos = move.PreviousPos;
        CurrentPos = move.CurrentPos;
        Capture = move.Capture;
        Check = move.Check;
        CheckMate = move.CheckMate;
        Algebraic = move.Algebraic;
    }
    public Piece Piece { get; set; }
    public Position PreviousPos { get; set; }
    public Position CurrentPos { get; set; }
    public bool Capture { get; set; }
    public bool Check { get; set; }
    public bool CheckMate { get; set; }
    public string Algebraic { get; set; }

    public object Clone()
    {
        var clone = new Move();
        clone.Piece = Piece;
        clone.PreviousPos = PreviousPos;
        clone.CurrentPos = CurrentPos;
        clone.Capture = Capture;
        clone.CheckMate = CheckMate;
        clone.Check = Check;
        clone.Algebraic = Algebraic;
        return clone;
    }

    public override string ToString()
    {
        return new
        {
            Piece = Piece,
            PreviousPos = PreviousPos,
            CurrentPos = CurrentPos,
            Capture = Capture,
            Check = Check,
            CheckMate = CheckMate,
            Algebric = Algebraic
        }.ToString();
    }
}


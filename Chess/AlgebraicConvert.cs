using System.Data;

namespace Chess;

public static class AlgebraicConvert
{
    public static Move AlgebraicToMove(string str, Board board, PieceColor color)
    {
        var algebraic = str;
        if (str == "O-O")
        {
            return new Move();
        }

        if (str == "O-O-O")
        {
            return new Move();
        }

        var result = new Move();

        str = new string(str.Where(x => x != '+' && x != '#' && x != 'x').ToArray());
        string str1 = str;

        Piece piece = new Piece();
        piece.Color = color;
        var oppositeColor = color is PieceColor.Black ? PieceColor.White : PieceColor.Black;

        if (char.IsUpper(str[0]))
        {
            piece.Type = str[0] switch
            {
                'K' => PieceType.King,
                'Q' => PieceType.Queen,
                'B' => PieceType.Bishop,
                'N' => PieceType.Knight,
                'R' => PieceType.Rook,
                _ => throw new ArgumentException()
            };

            str1 = str[Range.StartAt(1)];
        }
        else
        {
            piece.Type = PieceType.Pawn;
        }

        result.Piece = piece;

        var str2 = str1[Range.StartAt(str1.Length - 2)];
        var str3 = str1[Range.EndAt(str1.Length - 2)];

        result.CurrentPos = new Position(str2);

        var pos = new Position();

        var possiblePos = new List<Position>();

        for (int i = 0; i < 64; i++)
        {
            if (board.Pieces[i] is null) continue;
            if (board.Pieces[i].Type == piece.Type && board.Pieces[i].Color == color)
            {
                var legalMoves = LegalMoves.LegalMoveList(pos[i], board);
                if (legalMoves.Any(x => x.Index == result.CurrentPos.Index))
                {
                    possiblePos.Add(pos[i]);
                }
            }
        }

        if (str3.Length == 0)
        {
            if (possiblePos.Count != 1) throw new Exception();
            result.PreviousPos = possiblePos[0];
        }

        if (str3.Length == 1)
        {
            result.PreviousPos = possiblePos.First(x => x.Name.Contains(str3[0]));
        }

        if (str3.Length == 2)
        {
            result.PreviousPos = possiblePos.First(x => x.Name == str3);
        }

        var applyBoard = LegalMoves.ApplyMove(result.PreviousPos, result.CurrentPos, board);
        result.Check = Threat.IsChecked(applyBoard, oppositeColor);
        result.CheckMate = result.Check ? Threat.IsCheckMated(applyBoard, oppositeColor) : false;
        result.Capture = board.Pieces[result.CurrentPos.Index] is not null;
        result.Algebraic = ToAlgebraic(result.PreviousPos, result.CurrentPos, board);
        return result;
    }
    public static string ToAlgebraic(Position p1, Position p2, Board board)
    {
        var piece = board.Pieces[p1.Index];
        if (piece is null)
        {
            throw new ArgumentException();
        }
        var color = piece.Color;
        var oppositeColor = color is PieceColor.Black ? PieceColor.White : PieceColor.Black;

        var str = piece.Type switch
        {
            PieceType.Bishop => "B",
            PieceType.King => "K",
            PieceType.Knight => "N",
            PieceType.Queen => "Q",
            PieceType.Rook => "R",
            _ => ""
        };

        var pos = new Position();

        var possiblePos = new List<Position>();

        for (int i = 0; i < 64; i++)
        {
            if (board.Pieces[i] is null) continue;
            if (board.Pieces[i].Type == piece.Type && board.Pieces[i].Color == color)
            {
                var legalMoves = LegalMoves.LegalMoveList(pos[i], board);
                if (legalMoves.Any(x => x.Index == p2.Index))
                {
                    possiblePos.Add(pos[i]);
                }
            }
        }

        if (possiblePos.Count == 0)
        {
            throw new Exception("not valid move");
        }

        if (possiblePos.Count > 1)
        {
            if (possiblePos.Count(x => x.Column == p1.Column) == 0)
            {
                throw new Exception("not valid move");
            }
            if (possiblePos.Count(x => x.Column == p1.Column) == 1)
            {
                str += p1.Name[0];
            }
            else if (possiblePos.Count(x => x.Row == p1.Row) == 1)
            {
                str += p1.Name[1];
            }
            else
            {
                str += p1.Name;
            }
        }

        var enPassantCapture = false;
        if (piece.Type == PieceType.Pawn)
        {
            if (LegalMoves.PawnEnPassantMoves(p1, board).Any(x => x.Index == p2.Index))
            {
                enPassantCapture = true;
            }
        }
        if (board.Pieces[p2.Index] is not null || enPassantCapture)
        {
            if (board.Pieces[p1.Index].Type == PieceType.Pawn)
            {
                str += p1.Name[0];
            }
            str += "x";
        }

        str += p2.Name;
        var checkmate = Threat.IsCheckMated(LegalMoves.ApplyMove(p1, p2, board), oppositeColor);
        var check = Threat.IsChecked(LegalMoves.ApplyMove(p1, p2, board), oppositeColor);

        if (checkmate)
        {
            str += "#";
        }
        else
        {
            if (check) str += "+";
        }

        return str;
    }
}


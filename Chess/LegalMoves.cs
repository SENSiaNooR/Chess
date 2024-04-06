using System.Data;

namespace Chess;

public static class LegalMoves
{
    public static List<Position> PawnMoves(Position pos, Board board)
    {
        if (board.Pieces[pos.Index] is null) throw new Exception("empty position");
        if (board.Pieces[pos.Index].Type is not PieceType.Pawn) throw new Exception("pieceType isn't matched.");

        var color = board.Pieces[pos.Index].Color;
        var oppositeColor = color is PieceColor.Black ? PieceColor.White : PieceColor.Black;

        var sign = color switch
        {
            PieceColor.Black => -1,
            PieceColor.White => 1
        };
        var result = new List<Position>();

        try
        {
            var p1 = Position.Move(pos, sign, 0);
            if (board.Pieces[p1.Index] is null) result.Add(p1);
        }
        catch { }

        var starterIndex = color switch
        {
            PieceColor.Black => 6,
            PieceColor.White => 1
        };

        if (pos.Row == starterIndex)
        {
            var p1 = Position.Move(pos, sign, 0);
            var p2 = Position.Move(pos, 2 * sign, 0);
            if (board.Pieces[p2.Index] is null && board.Pieces[p1.Index] is null) result.Add(p2);
        }

        result = result.Concat(PawnCaptureMoves(pos, board)).Concat(PawnEnPassantMoves(pos, board)).ToList();

        return result;
    }

    public static List<Position> PawnCaptureMoves(Position pos, Board board)
    {
        if (board.Pieces[pos.Index] is null) throw new Exception("empty position");
        if (board.Pieces[pos.Index].Type is not PieceType.Pawn) throw new Exception("pieceType isn't matched.");

        var color = board.Pieces[pos.Index].Color;
        var oppositeColor = color is PieceColor.Black ? PieceColor.White : PieceColor.Black;

        var sign = color switch
        {
            PieceColor.Black => -1,
            PieceColor.White => 1
        };
        var result = new List<Position>();

        try
        {
            var p3 = Position.Move(pos, sign, -1);
            if (board.Pieces[p3.Index] is not null)
            {
                if (board.Pieces[p3.Index].Color != color) result.Add(p3);
            }
        }
        catch { }

        try
        {
            var p4 = Position.Move(pos, sign, 1);
            if (board.Pieces[p4.Index] is not null)
            {
                if (board.Pieces[p4.Index].Color != color) result.Add(p4);
            }
        }
        catch { }

        return result;
    }

    public static List<Position> PawnEnPassantMoves(Position pos, Board board)
    {
        if (board.Pieces[pos.Index] is null) throw new Exception("empty position");
        if (board.Pieces[pos.Index].Type is not PieceType.Pawn) throw new Exception("pieceType isn't matched.");

        var color = board.Pieces[pos.Index].Color;
        var oppositeColor = color is PieceColor.Black ? PieceColor.White : PieceColor.Black;

        var sign = color switch
        {
            PieceColor.Black => -1,
            PieceColor.White => 1
        };
        var result = new List<Position>();

        var EnPassantRowIndex = color switch
        {
            PieceColor.Black => 3,
            PieceColor.White => 4
        };

        try
        {
            if (pos.Row == EnPassantRowIndex)
            {
                if (board.Moves.Count > 0)
                {
                    var lastMove = board.Moves.Last();
                    if (lastMove.Piece.Type == PieceType.Pawn)
                    {
                        if (Math.Abs(lastMove.CurrentPos.Row - lastMove.PreviousPos.Row) == 2)
                        {
                            if (lastMove.CurrentPos.Column - pos.Column == 1)
                            {
                                var p5 = Position.Move(pos, sign, 1);
                                result.Add(p5);
                            }
                            if (lastMove.CurrentPos.Column - pos.Column == -1)
                            {
                                var p6 = Position.Move(pos, sign, -1);
                                result.Add(p6);
                            }
                        }
                    }
                }
            }
        }
        catch { }

        return result;
    }

    public static List<Position> RookMoves(Position pos, Board board)
    {
        if (board.Pieces[pos.Index] is null) throw new Exception("empty position");
        if (board.Pieces[pos.Index].Type is not PieceType.Rook) throw new Exception("pieceType isn't matched.");

        var color = board.Pieces[pos.Index].Color;
        var oppositeColor = color is PieceColor.Black ? PieceColor.White : PieceColor.Black;

        var result = new List<Position>();

        var range1 = Enumerable.Range(1, 7);
        var repeat = Enumerable.Repeat(0, 7);
        var range2 = Enumerable.Range(-7, 7).Reverse();
        var list1 = range1.Zip(repeat);
        var list2 = repeat.Zip(range1);
        var list3 = range2.Zip(repeat);
        var list4 = repeat.Zip(range2);
        var lists = new List<IEnumerable<(int, int)>>()
        {
            list1,list2,list3,list4
        };
        foreach (var list in lists)
        {
            foreach (var item in list)
            {
                if (Position.TryMove(pos, item.Item1, item.Item2, out var outPos))
                {
                    if (board.Pieces[outPos.Index] is null)
                    {
                        result.Add(outPos);
                        continue;
                    }
                    if (board.Pieces[outPos.Index].Color == oppositeColor)
                    {
                        result.Add(outPos);
                    }
                    break;
                }
            }
        }

        return result;
    }

    public static List<Position> KnightMoves(Position pos, Board board)
    {
        if (board.Pieces[pos.Index] is null) throw new Exception("empty position");
        if (board.Pieces[pos.Index].Type is not PieceType.Knight) throw new Exception("pieceType isn't matched.");

        var color = board.Pieces[pos.Index].Color;
        var oppositeColor = color is PieceColor.Black ? PieceColor.White : PieceColor.Black;

        var result = new List<Position>();

        var xyKnightMoves = new List<(int x, int y)>()
        {
            (2,1),(2,-1),(-2,1),(-2,-1),(1,2),(1,-2),(-1,2),(-1,-2)
        };
        foreach (var xy in xyKnightMoves)
        {
            if (Position.TryMove(pos, xy.x, xy.y, out var outPos))
            {
                if (board.Pieces[outPos.Index] is null)
                {
                    result.Add(outPos);
                    continue;
                }

                if (board.Pieces[outPos.Index].Color == oppositeColor)
                {
                    result.Add(outPos);
                }
            }
        }

        return result;
    }

    public static List<Position> BishopMoves(Position pos, Board board)
    {
        if (board.Pieces[pos.Index] is null) throw new Exception("empty position");
        if (board.Pieces[pos.Index].Type is not PieceType.Bishop) throw new Exception("pieceType isn't matched.");

        var color = board.Pieces[pos.Index].Color;
        var oppositeColor = color is PieceColor.Black ? PieceColor.White : PieceColor.Black;

        var result = new List<Position>();

        var range1 = Enumerable.Range(1, 7);
        var range2 = Enumerable.Range(-7, 7).Reverse();
        var list1 = range1.Zip(range2);
        var list2 = range2.Zip(range1);
        var list3 = range1.Zip(range1);
        var list4 = range2.Zip(range2);
        var lists = new List<IEnumerable<(int, int)>>()
        {
            list1,list2,list3,list4
        };
        foreach (var list in lists)
        {
            foreach (var item in list)
            {
                if (Position.TryMove(pos, item.Item1, item.Item2, out var outPos))
                {
                    if (board.Pieces[outPos.Index] is null)
                    {
                        result.Add(outPos);
                        continue;
                    }
                    if (board.Pieces[outPos.Index].Color == oppositeColor)
                    {
                        result.Add(outPos);
                    }
                    break;
                }
            }
        }

        return result;
    }

    public static List<Position> QueenMoves(Position pos, Board board)
    {
        if (board.Pieces[pos.Index] is null) throw new Exception("empty position");
        if (board.Pieces[pos.Index].Type is not PieceType.Queen) throw new Exception("pieceType isn't matched.");

        var color = board.Pieces[pos.Index].Color;
        var oppositeColor = color is PieceColor.Black ? PieceColor.White : PieceColor.Black;

        var result = new List<Position>();

        var range1 = Enumerable.Range(1, 7);
        var range2 = Enumerable.Range(-7, 7).Reverse();
        var repeat = Enumerable.Repeat(0, 7);
        var list1 = range1.Zip(repeat);
        var list2 = range1.Zip(range1);
        var list3 = repeat.Zip(range1);
        var list4 = range2.Zip(range1);
        var list5 = range2.Zip(repeat);
        var list6 = range2.Zip(range2);
        var list7 = repeat.Zip(range2);
        var list8 = range1.Zip(range2);
        var lists = new List<IEnumerable<(int, int)>>()
        {
            list1,list2, list3, list4, list5, list6, list7, list8
        };
        foreach (var list in lists)
        {
            foreach (var item in list)
            {
                if (Position.TryMove(pos, item.Item1, item.Item2, out var outPos))
                {
                    if (board.Pieces[outPos.Index] is null)
                    {
                        result.Add(outPos);
                        continue;
                    }
                    if (board.Pieces[outPos.Index].Color == oppositeColor)
                    {
                        result.Add(outPos);
                    }
                    break;
                }
            }
        }
        return result;
    }

    public static List<Position> KingMoves(Position pos, Board board)
    {
        if (board.Pieces[pos.Index] is null) throw new Exception("empty position");
        if (board.Pieces[pos.Index].Type is not PieceType.King) throw new Exception("pieceType isn't matched.");

        var color = board.Pieces[pos.Index].Color;
        var oppositeColor = color is PieceColor.Black ? PieceColor.White : PieceColor.Black;

        Predicate<Piece?> predicate = piece =>
        {
            if (piece is null) return false;
            if (piece.Type is PieceType.King && piece.Color == oppositeColor) return true;
            return false;
        };

        var oppositeColorKingPos = pos[board.Pieces.ToList().FindIndex(predicate)];

        var result = new List<Position>();

        var xyKingMoves = new List<(int x, int y)>()
        {
            (1,1),(0,1),(-1,1),(-1,0),(-1,-1),(0,-1),(1,-1),(1,0)
        };

        var invalidPositions = new List<Position>();

        foreach (var xy in xyKingMoves)
        {
            if (Position.TryMove(oppositeColorKingPos, xy.x, xy.y, out var outPos))
            {
                invalidPositions.Add(outPos);
            }
        }
        foreach (var xy in xyKingMoves)
        {
            if (Position.TryMove(pos, xy.x, xy.y, out var outPos))
            {
                if (board.Pieces[outPos.Index] is null)
                {
                    result.Add(outPos);
                    continue;
                }
                if (board.Pieces[outPos.Index].Color == oppositeColor)
                {
                    if (board.Pieces[outPos.Index].Type != PieceType.King)
                    {
                        result.Add(outPos);
                    }
                }
            }
        }

        var invalidPositionsName = invalidPositions.Select(p => p.Name);

        return result.ExceptBy(invalidPositionsName, x => x.Name).ToList();
    }

    public static Board ApplyMove(Position previousPos, Position currentPos, Board board)
    {
        if (board.Pieces[previousPos.Index] is null) throw new ArgumentNullException();
        var newBoard = (Board)board.Clone();
        var piece = (Piece)newBoard.Pieces[previousPos.Index].Clone();

        if (piece.Type == PieceType.Pawn)
        {
            if (PawnEnPassantMoves(previousPos, board).Any(x => x.Index == currentPos.Index))
            {
                if (piece.Color == PieceColor.White)
                {
                    newBoard.Pieces[Position.Move(currentPos, -1, 0).Index] = null;
                }
                else
                {
                    newBoard.Pieces[Position.Move(currentPos, 1, 0).Index] = null;
                }
            }
        }

        newBoard.Pieces[previousPos.Index] = null;
        newBoard.Pieces[currentPos.Index] = piece;


        return newBoard;
    }

    public static List<Position> RemoveCheckedPositions(Position pos, Board board, List<Position> positions)
    {
        if (board.Pieces[pos.Index] is null) throw new ArgumentNullException();

        var color = board.Pieces[pos.Index].Color;
        var result = new List<Position>();
        foreach (var position in positions)
        {
            var applyBoard = ApplyMove(pos, position, board);
            if (Threat.IsChecked(applyBoard, color))
            {
                continue;
            }
            result.Add(position);
        }
        return result;
    }

    public static List<Position> LegalMoveList(Position pos, Board board)
    {
        if (board.Pieces[pos.Index] is null) throw new ArgumentNullException();

        var pieceType = board.Pieces[pos.Index].Type;
        var result = pieceType switch
        {
            PieceType.King => KingMoves(pos, board),
            PieceType.Queen => QueenMoves(pos, board),
            PieceType.Rook => RookMoves(pos, board),
            PieceType.Knight => KnightMoves(pos, board),
            PieceType.Bishop => BishopMoves(pos, board),
            PieceType.Pawn => PawnMoves(pos, board),
        };
        return RemoveCheckedPositions(pos, board, result);
    }
}


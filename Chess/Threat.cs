using System.Data;

namespace Chess;

public static class Threat
{
    private static List<List<int>> CheckableRangeCache => CheckableRangeFileIO();

    public static List<Position> Threats(Board board, PieceColor color, IEnumerable<int>? IndexRange = null)
    {
        if (IndexRange is null)
        {
            IndexRange = Enumerable.Range(0, 64);
        }
        var pos = new Position();
        var result = new List<Position>();
        foreach (var i in IndexRange)
        {
            if (board.Pieces[i] is null) continue;
            if (board.Pieces[i].Color == color)
            {
                var threatPositions = board.Pieces[i].Type switch
                {
                    PieceType.Queen => LegalMoves.QueenMoves(pos[i], board),
                    PieceType.Rook => LegalMoves.RookMoves(pos[i], board),
                    PieceType.Knight => LegalMoves.KnightMoves(pos[i], board),
                    PieceType.Bishop => LegalMoves.BishopMoves(pos[i], board),
                    PieceType.Pawn => LegalMoves.PawnCaptureMoves(pos[i], board),
                    _ => new List<Position>()
                };
                result.AddRange(threatPositions);
            }
        }
        return result;
    }

    public static bool IsChecked(Board board, PieceColor color)
    {
        Predicate<Piece?> predicate = piece =>
        {
            if (piece is null) return false;
            if (piece.Type is PieceType.King && piece.Color == color) return true;
            return false;
        };
        var pos = new Position();
        var kingPosIndex = board.Pieces.ToList().FindIndex(predicate);
        var oppositeColor = color is PieceColor.Black ? PieceColor.White : PieceColor.Black;
        return Threats(board, oppositeColor, CheckableRange(pos[kingPosIndex])).Any(x => x.Index == kingPosIndex);
    }

    public static bool IsCheckMated(Board board, PieceColor color)
    {
        if (!IsChecked(board, color)) return false;

        Predicate<Piece?> predicate = piece =>
        {
            if (piece is null) return false;
            if (piece.Type is PieceType.King && piece.Color == color) return true;
            return false;
        };
        var pos = new Position();
        var kingPos = pos[board.Pieces.ToList().FindIndex(predicate)];
        var kingMoves = LegalMoves.LegalMoveList(kingPos, board);

        if (kingMoves.Count > 0) return false;

        var canEscapeCheckWithOtherPiece = board.Pieces
            .ToList()
            .Select((x, index) => (x, index))
            .Where(y => y.x is not null)
            .Where(y => y.x.Color == color)
            .Select(y => y.index)
            .Any(i => LegalMoves.LegalMoveList(pos[i], board).Count > 0);

        if (canEscapeCheckWithOtherPiece) return false;

        return true;
    }
    
    private static IEnumerable<int> CheckableRange(Position pos) => CheckableRangeCache[pos.Index];

    private static List<List<int>> CheckableRangeFileIO()
    {
        string str;
        try
        {
            str = File.ReadAllText("CheckableRange.txt");
        }
        catch
        {
            var pos = new Position();
            var allPosCheckRange = new List<List<int>>();

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
            var list9 = new List<(int, int)> { (2, 1), (2, -1), (-2, 1), (-2, -1), (1, 2), (1, -2), (-1, 2), (-1, -2) };
            var lists = new List<IEnumerable<(int, int)>>
            { 
                list1,list2, list3, list4, list5, list6, list7, list8, list9
            };
            for (int i = 0; i < 64; i++)
            {
                var res = new List<int>();
                foreach (var list in lists)
                {
                    foreach (var item in list)
                    {
                        if (Position.TryMove(pos[i], item.Item1, item.Item2, out var outPos))
                        {
                            res.Add(outPos.Index);
                        }
                    }
                }
                allPosCheckRange.Add(res);
            }
            str = string.Join('\n',allPosCheckRange.Select(x => string.Join(',', x)));
            File.AppendAllText("CheckableRange.txt",str);
        }
        var allPos = str.Split('\n').ToList().Select(x => x.Split(',').Select(i => Convert.ToInt32(i)).ToList()).ToList();
        return allPos;
    }

}


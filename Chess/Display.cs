using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess;
public class DisplayBoard
{
    public bool ClearBeforeDisplay { get; set; } = true;
    public int DistanceFromTop { get; set; } = 1;
    public int DistanceFromLeft { get; set; } = 2;
    public ConsoleColor DarkColor { get; set; } = ConsoleColor.DarkGreen;
    public ConsoleColor LightColor { get; set; } = ConsoleColor.DarkYellow;
    public ConsoleColor HighLightColor1 { get; set; } = ConsoleColor.Gray;
    public ConsoleColor HighLightColor2 { get; set; } = ConsoleColor.DarkMagenta;
    public ConsoleColor CheckColor { get; set; } = ConsoleColor.DarkRed;
    public ConsoleColor WhiteColor { get; set; } = ConsoleColor.White;
    public ConsoleColor BlackColor { get; set; } = ConsoleColor.Black;
    public bool DisplayMovesList { get; set; } = true;
    public bool ShowNotation { get; set; } = true;
    public bool FlipBoard { get; set; } = false;

    public List<Position> HighLight1Positions { get; set; } = new List<Position>();
    public Position? HighLight2Position { get; set; } = null;

    public enum Align
    {
        TopLeft,
        Center
    }

    public Align CharAlign { get; set; } = Align.Center;

    public void Display(Board board)
    {
        if (ClearBeforeDisplay) Console.Clear();
        if (ShowNotation)
        {
            var charList = new List<char>()
                { '1', '2', '3', '4', '5', '6', '7', '8', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            foreach (var c in charList)
            {
                var p1 = NotationCellPosition1(c);
                var p2 = NotationCellPosition2(c);
                CellPrint(p1.left + DistanceFromLeft, p1.top + DistanceFromTop, c, ConsoleColor.Black, WhiteColor, false, false);
                CellPrint(p2.left + DistanceFromLeft, p2.top + DistanceFromTop, c, ConsoleColor.Black, WhiteColor, false, false);
            }
        }

        for (int i = 0; i < 64; i++)
        {
            var c = ' ';
            var forColor = WhiteColor;
            bool check = false;
            bool highLight1 = HighLight1Positions.Any(x => x.Index == i);
            bool highLight2 = false;
            if (HighLight2Position is not null)
            {
                highLight2 = HighLight2Position.Index == i;
            }

            if (board.Pieces[i] is not null)
            {
                c = board.Pieces[i].Type switch
                {
                    PieceType.Bishop => 'B',
                    PieceType.King => 'K',
                    PieceType.Knight => 'N',
                    PieceType.Pawn => 'P',
                    PieceType.Queen => 'Q',
                    PieceType.Rook => 'R'
                };
                forColor = board.Pieces[i].Color is PieceColor.White ? WhiteColor : BlackColor;
                check = (c == 'K' && board.Situations[board.Pieces[i].Color].IsChecked);
            }
            var p = CellPosition(i);
            CellPrint(p.left + DistanceFromLeft , p.top + DistanceFromTop , c,  CellBackColor(i) ,forColor ,check , highLight1, highLight2);
        }

        if (DisplayMovesList)
        {
            var moves = MovesAlgebraic(board);
            Console.ResetColor();
            for (int i = 0; i < moves.Count; i++)
            {
                var p3 = MovesPosition(i);
                Console.SetCursorPosition(p3.left, p3.top);
                Console.Write(moves[i]);
            }
        }
        if (ShowNotation) Console.SetCursorPosition(0 , 31 + DistanceFromTop);
        else Console.SetCursorPosition(0, 25 + DistanceFromTop);
    }

    private List<string> MovesAlgebraic(Board board)
    {
        var result = board.Moves
            .ToList()
            .Select(x => x.Algebraic)
            .Chunk(2)
            .Select(x => string.Join("  ", x.ToList()))
            .ToList();

        result = result
            .Select((x, index) => (index + 1).ToString() + "." + x)
            .ToList();

        if (result.Count > 48)
        {
            result = result.TakeLast(48).ToList();
        }
        return result;
    }
    private (int left, int top) MovesPosition(int index)
    {
        var left = 56 + DistanceFromLeft;
        var top = DistanceFromTop;
        if (ShowNotation)
        {
            left += 12;
            top += 3;
        }

        if (index >= 24)
        {
            left += 18;
        }

        top += index % 24;
        return (left, top);
    }
    private (int left, int top) CellPosition(int index)
    {
        var pos = new Position();
        int top;
        int left;
        if (FlipBoard)
        {
            top = pos[index].Row;
            left = 7 - pos[index].Column;
        }
        else
        {
            top = 7 - pos[index].Row;
            left = pos[index].Column;
        }
        top *= 3;
        left *= 6;
        if (ShowNotation)
        {
            top += 3;
            left += 6;
        }
        return (left, top);
    }
    private ConsoleColor CellBackColor(int index)
    {
        if (((index / 8) + (index % 8)) % 2 == 0)
        {
            return DarkColor;
        }

        return LightColor;
    }
    private (int left, int top) NotationCellPosition1(char c)
    {
        int top;
        int left;
        if (char.IsNumber(c))
        {
            if (FlipBoard)
            {
                top = (int)(c - '0');
            }
            else
            {
                top = 9 - (int)(c - '0'); 
            }
            left = 0;
            top *= 3;
            return (left, top);
        }
        top = 0;
        if (FlipBoard)
        {
            left = 8 - (int)(c - 'a');
        }
        else
        {
            left = (int)(c - 'a') + 1;
        }
        left *= 6;
        return (left, top);

    }
    private (int left, int top) NotationCellPosition2(char c)
    {
        int top;
        int left;
        if (char.IsNumber(c))
        {
            if (FlipBoard)
            {
                top = (int)(c - '0');
            }
            else
            {
                top = 9 - (int)(c - '0');
            }
            left = 54;
            top *= 3;
            return (left, top);
        }
        top = 27;
        if (FlipBoard)
        {
            left = 8 - (int)(c - 'a');
        }
        else
        {
            left = (int)(c - 'a') + 1;
        }
        left *= 6;
        return (left, top);
    }
    private void CellPrint(int left, int top, char c,ConsoleColor backColor ,ConsoleColor forColor , bool check = false, bool highlight1 = false, bool highlight2 = false)
    {
        Console.BackgroundColor = backColor;
        Console.ForegroundColor = forColor;
        Console.SetCursorPosition(left ,top);
        List<string> str = new List<string>();
        if (CharAlign is Align.Center)
        {
            str.Add("      ");
            str.Add("  " + c + "   ");
            str.Add("      ");
        }
        else
        {
            str.Add(c + "     ");
            str.Add("      ");
            str.Add("      ");
        }

        Console.Write(str[0]);
        Console.SetCursorPosition(left, top + 1);
        if (check || highlight1 || highlight2)
        {
            Console.Write(str[1][Range.EndAt(2)]);
            if (check) Console.BackgroundColor = CheckColor;
            if (highlight1) Console.BackgroundColor = HighLightColor1;
            if (highlight2) Console.BackgroundColor = HighLightColor2;
            Console.Write(str[1][Range.StartAt(2)][Range.EndAt(2)]);
            Console.BackgroundColor = backColor;
            Console.Write(str[1][Range.StartAt(4)]);
        }
        else
        {
            Console.Write(str[1]);
        }
        Console.SetCursorPosition(left, top + 2);
        Console.Write(str[2]);
        Console.ResetColor();
    }
}



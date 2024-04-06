using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess;

public class GameManager
{
    public DisplayBoard Display { get; set; }
    public Board GameBoard { get; set; }
    public string Player1Name { get; set; } = "";
    public string Player2Name { get; set; } = "";
    public bool FlipBoardEachMove { get; set; } = false;
    public PlayMethod HowToPlay { get; set; } = PlayMethod.ArrowKey;
    public PieceColor Turn { get; set; } = PieceColor.White;
    private PieceColor? Loser;

    public enum PlayMethod
    {
        Typing,
        ArrowKey
    }
    public GameManager(string player1Name, string player2Name)
    {
        Display = new DisplayBoard();
        GameBoard = new Board();
        Player1Name = player1Name;
        Player2Name = player2Name;
    }

    public GameManager()
    {
        Display = new DisplayBoard();
        GameBoard = new Board();
    }

    public void StartAutoPlay()
    {
        if (Player1Name == "" || Player2Name == "")
        {
            SetPlayersName();
        }
        var checkmate = false;
        while (!checkmate)
        {
            DisplayGameBoard();
            if (HowToPlay == PlayMethod.Typing)
            {
                GetMoveByTyping();
            }
            else
            {
                GetMoveByArrowKey();
            }
            ChangeTurn();
            if (GameBoard.Situations[Turn].IsCheckMated)
            {
                checkmate = true;
            }
            Loser = Turn;
        }
        Display.Display(GameBoard);
        GameReport();
    }
    
    public void StartManualPlay()
    {
        if (Player1Name == "" || Player2Name == "")
        {
            SetPlayersName();
        }
        DisplayGameBoard();
    }
    
    public void NextMove(string algebraicMove)
    {
        GameBoard = GameBoard.Move(algebraicMove, Turn);
        DisplayGameBoard();
        ChangeTurn();

        if (GameBoard.Situations[Turn].IsCheckMated)
        {
            Loser = Turn;
            GameReport();   
        }
    }

    private void SetPlayersName()
    {
        Console.Clear();
        Console.WriteLine("Enter Player1 Name:");
        Player1Name = Console.ReadLine();
        Console.WriteLine("Enter Player2 Name:");
        Player2Name = Console.ReadLine();
        Console.Clear();
    }

    private void GetMoveByTyping()
    {
        var str = Turn == PieceColor.White ? $"{Player1Name}(White) turn!" : $"{Player2Name}(Black) turn!";
        str += "\nType your move:";
        var success = false;
        while (!success)
        {
            Console.WriteLine(str);
            var algebraic = Console.ReadLine();
            try
            {
                GameBoard = GameBoard.Move(algebraic, Turn);
                success = true;
            }
            catch 
            {
                Console.WriteLine("\nInvalid move! Try again.");
            }
        }
    }

    private void GetMoveByArrowKey()
    {
        var startPos = new Position();
        var pos = Display.FlipBoard ? new Position(7, 7) : new Position(0, 0);
        var sign = Display.FlipBoard ? -1 : 1;
        var callMethodAgain = false;
        Display.ClearBeforeDisplay = false;
        while (true)
        {
            Display.HighLight2Position = pos;
            Console.SetCursorPosition(0,0);
            Display.Display(GameBoard);
            var str = Turn == PieceColor.White ? $"{Player1Name}(White) turn!" : $"{Player2Name}(Black) turn!";
            str += "\nUse arrow key to move around the board. Then use Enter to comit piece for move";
            Console.WriteLine(str);
            var key = Console.ReadKey();
            try
            {
                pos = key.Key switch
                {
                    ConsoleKey.UpArrow => Position.Move(pos, 1 * sign, 0),
                    ConsoleKey.DownArrow => Position.Move(pos, -1 * sign, 0),
                    ConsoleKey.LeftArrow => Position.Move(pos, 0, -1 * sign),
                    ConsoleKey.RightArrow => Position.Move(pos, 0, 1 * sign),
                    _ => pos
                };
            }
            catch { }

            if (key.Key == ConsoleKey.Enter)
            {
                if (GameBoard.Pieces[pos.Index] is null) continue;
                if (GameBoard.Pieces[pos.Index].Color != Turn) continue;
                if (LegalMoves.LegalMoveList(pos, GameBoard).Count == 0 ) continue;

                startPos = new Position(pos.Index);
                break;
            }
        }

        Display.HighLight1Positions = LegalMoves.LegalMoveList(pos, GameBoard);
        Console.Clear();
        while (true)
        {
            Display.HighLight2Position = pos;
            Console.SetCursorPosition(0, 0);
            Display.Display(GameBoard);
            var str = Turn == PieceColor.White ? $"{Player1Name}(White) turn!" : $"{Player2Name}(Black) turn!";
            str += "\nUse arrow key to move around the board. Then use Enter to select move or Escape to back";
            Console.WriteLine(str);
            var key = Console.ReadKey();
            try
            {
                pos = key.Key switch
                {
                    ConsoleKey.UpArrow => Position.Move(pos, 1 * sign, 0),
                    ConsoleKey.DownArrow => Position.Move(pos, -1 * sign, 0),
                    ConsoleKey.LeftArrow => Position.Move(pos, 0, -1 * sign),
                    ConsoleKey.RightArrow => Position.Move(pos, 0, 1 * sign),
                    _ => pos
                };
            }
            catch { }

            if (key.Key == ConsoleKey.Enter)
            {
                if (Display.HighLight1Positions.Any(x => x.Index == pos.Index))
                {
                    Display.ClearBeforeDisplay = true;
                    GameBoard = GameBoard.Move(AlgebraicConvert.ToAlgebraic(startPos, pos, GameBoard), Turn);
                    Display.HighLight1Positions.Clear();
                    Display.HighLight2Position = null;
                    return;
                }
                continue;
            }
            if (key.Key == ConsoleKey.Escape) 
            { 
                callMethodAgain = true;
                break;
            }
        }
        Display.HighLight1Positions.Clear();
        Display.HighLight2Position = null;
        GetMoveByArrowKey();
    }

    private void DisplayGameBoard()
    {
        if (FlipBoardEachMove)
        {
            Display.FlipBoard = Turn == PieceColor.White ? false : true;
        }
        Display.Display(GameBoard);
    }

    private void ChangeTurn()
    {
        Turn = Turn is PieceColor.Black ? PieceColor.White : PieceColor.Black;
    }

    private void GameReport()
    {
        var winnerName = Loser == PieceColor.White ? Player2Name : Player1Name;
        var winnerColor = Loser == PieceColor.White ? "Black" : "White";
        Console.WriteLine();
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine($"Player {winnerName} with {winnerColor} color won the game!");
        Console.ResetColor();
    }
}


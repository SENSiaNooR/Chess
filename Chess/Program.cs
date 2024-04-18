using System.Drawing;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chess;
public class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("FullScreen first and then press any key");
        Console.ReadKey();
        Console.Clear();
        var game = new GameManager();
        game.FlipBoardEachMove = true;
        game.HowToPlay = GameManager.PlayMethod.ArrowKey;
        game.Display.HowToDisplayPieces = DisplayBoard.TypoGraph.Symbol;
        game.StartAutoPlay();
    }
}



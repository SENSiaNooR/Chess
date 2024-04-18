using System.Drawing;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace Chess;
public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("FullScreen first and then press any key");
        Console.ReadKey();
        Console.Clear();
        var game = new GameManager();
        game.FlipBoardEachMove = true;
        game.HowToPlay = GameManager.PlayMethod.ArrowKey;
        game.StartAutoPlay();
    }
}



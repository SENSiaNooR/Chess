using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess;

public enum PieceType
{
    Pawn,
    Rook,
    Bishop,
    Knight,
    Queen,
    King
}

public enum PieceColor
{
    White,
    Black
}
public class Piece : ICloneable
{
    public PieceType Type;
    public PieceColor Color;

    public static Dictionary<PieceType, char> PieceTypeAbb = new()
    {
        {PieceType.Pawn , 'P'},
        {PieceType.Rook , 'R'},
        {PieceType.Bishop ,'B'},
        {PieceType.Knight ,'N'},
        {PieceType.Queen ,'Q'},
        {PieceType.King ,'K'}
    };

    public object Clone()
    {
        return new Piece { Color = this.Color, Type= this.Type};
    }

    public override string ToString()
    {
        return $"[Type : {Type} , Color : {Color}]";
    }
}


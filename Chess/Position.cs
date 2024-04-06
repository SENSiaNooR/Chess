using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chess;

public class Position : IEquatable<Position?> , IEqualityComparer<Position?> , ICloneable
{ 
    public int Row;
    public int Column;
    public int Index;
    public string Name;
    private static readonly Position[] _defaultPositions = Enumerable.Range(0, 64).Select(x => new Position(x)).ToArray();

    public Position this[int x] => _defaultPositions[x];

    public Position this[string x] => _defaultPositions[ConvertToIndex(x)];

    public Position this[int row, int column] => _defaultPositions[ConvertToIndex(row, column)];

    public Position()
    {
        Row = 0;
        Column = 0;
        Index = 0;
        Name = "a1";
    }

    public Position(int row, int column)
    {
        if (row < 0 || column < 0 || row > 7 || column > 7) 
            throw new ArgumentOutOfRangeException();
        Row = row;
        Column = column;
        Name = ConvertToString(row, column);
        Index = ConvertToIndex(row, column);
    }
    public static bool TryCreatePosition(int row, int column, out Position? position)
    {
        if (row < 0 || column < 0 || row > 7 || column > 7)
        {
            position = default;
            return false;
        }
        position = new Position(row, column);
        return true;
    }
    public Position(string name)
    {
        var pos = ConvertToRowAndColumn(name);
        if (pos.row < 0 || pos.column < 0 || pos.row > 7 || pos.column > 7) 
            throw new ArgumentOutOfRangeException();
        Row = pos.row;
        Column = pos.column;
        Name = name;
        Index = ConvertToIndex(pos.row, pos.column);
    }

    public Position(int index)
    {
        var pos = ConvertToRowAndColumn(index);
        if (pos.row < 0 || pos.column < 0 || pos.row > 7 || pos.column > 7) 
            throw new ArgumentOutOfRangeException();
        Row = pos.row;
        Column = pos.column;
        Index = index;
        Name = ConvertToString(pos.row, pos.column);
    }

    private static (int row, int column) ConvertToRowAndColumn(string name)
    {
        if (name.Length is not 2) 
            throw new ArgumentOutOfRangeException(nameof(name));
        var column = (int)(name[0] - 97);
        var row = (int)(name[1] - 48) - 1;
        return (row, column);
    }
    private static (int row, int column) ConvertToRowAndColumn(int index)
    {
        int row = index / 8;
        int column = index % 8;
        return (row, column);
    }

    private static string ConvertToString(int row, int column)
    {
        if (column is < 0 or > 7) throw new ArgumentOutOfRangeException(nameof(column));
        if (row is < 0 or > 7) throw new ArgumentOutOfRangeException(nameof(row));
        var col = (char)(column + 97);
        var result = col + (row + 1).ToString();
        return result;
    }

    private static int ConvertToIndex(int row, int column)
    {
        return row * 8 + column;
    }

    private static int ConvertToIndex(string name)
    {
        var pos = ConvertToRowAndColumn(name);
        return ConvertToIndex(pos.row, pos.column);
    }

    public static Position Move(Position pos, int up, int right)
    {
        var newRow = pos.Row + up;
        var newColumn = pos.Column + right;
        var index = ConvertToIndex(newRow , newColumn);
        if (newRow < 0 || newColumn < 0 || newRow > 7 || newColumn > 7)
        {
            throw new ArgumentOutOfRangeException();
        }
        return _defaultPositions[index];
    }

    public static bool TryMove(Position pos, int up, int right, out Position? outPos)
    {
        var newRow = pos.Row + up;
        var newColumn = pos.Column + right;
        var index = ConvertToIndex(newRow, newColumn);
        if (newRow < 0 || newColumn < 0 || newRow > 7 || newColumn > 7)
        {
            outPos = null;
            return false;
        }
        outPos = _defaultPositions[index];
        return true;
    }

    public override string ToString()
    {
        return new
        {
            Name = Name,
            Row = Row,
            Column = Column,
            Index = Index
        }.ToString();
    }

    public bool Equals(Position? other)
    {
        if (other == null) return false;
        if (Row == other.Row && Column == other.Column) return true;
        return false;
    }

    public bool Equals(Position? x, Position? y)
    {
        if (x == null) return false;
        if (y == null) return false;
        if (x.Row == y.Row && x.Column == y.Column) return true;
        return false;
    }

    public int GetHashCode([DisallowNull] Position? obj)
    {
        return obj.Name.GetHashCode();
    }

    public object Clone()
    {
        return new Position(this.Index);
    }
}

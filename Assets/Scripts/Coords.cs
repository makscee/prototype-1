using UnityEngine;

public class Coords
{
    Coords(){}

    Coords(TetrisField field)
    {
        _field = field;
    }

    Coords(TetrisField field, int x, int y)
    {
        X = x;
        Y = y;
        _field = field;
    }

    readonly TetrisField _field;

    public int Y { get; private set; }
    public int X { get; private set; }

    public int GlobalX => (int)(_field.BlockSide) * X;
    public int GlobalY => (int)(_field.BlockSide) * Y;

    public override string ToString()
    {
        return $"({X}, {Y})";
    }


    public static Coords FromIndices(TetrisField field, int x, int y)
    {
        return new Coords(field, x, y);
    }
}
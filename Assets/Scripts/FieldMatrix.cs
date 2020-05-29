using System.Collections.Generic;

public static class FieldMatrix
{
    static readonly Dictionary<int, Dictionary<int, Block>> Matrix = new Dictionary<int, Dictionary<int, Block>>();

    public static void Add(int x, int y, Block block)
    {
        if (!Matrix.ContainsKey(x)) Matrix[x] = new Dictionary<int, Block>();
        Matrix[x][y] = block;
    }

    public static Block Get(int x, int y)
    {
        return Matrix.ContainsKey(x) ? (Matrix[x].ContainsKey(y) ? Matrix[x][y] : null) : null;
    }

    public static void Clear(int x, int y)
    {
        if (Matrix.ContainsKey(x)) Matrix[x].Remove(y);
    }
}
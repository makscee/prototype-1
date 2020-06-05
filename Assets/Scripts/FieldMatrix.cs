using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

public static class FieldMatrix
{
    static readonly Dictionary<int, Dictionary<int, Block>> Matrix = new Dictionary<int, Dictionary<int, Block>>();

    public static bool Add(int x, int y, Block block)
    {
        if (!Matrix.ContainsKey(x)) Matrix[x] = new Dictionary<int, Block>();
        if (Matrix.ContainsKey(x) && Matrix[x].ContainsKey(y)) return false;
        Matrix[x][y] = block;
        return true;
    }

    public static bool Get(int x, int y, out Block block)
    {
        if (Matrix.ContainsKey(x) && Matrix[x].ContainsKey(y))
        {
            block = Matrix[x][y];
            return true;
        }

        block = null;
        return false;
    }

    public static void Clear(int x, int y)
    {
        if (Matrix.ContainsKey(x)) Matrix[x].Remove(y);
    }
}
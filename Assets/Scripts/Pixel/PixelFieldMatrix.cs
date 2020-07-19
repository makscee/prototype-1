using System.Collections.Generic;
using UnityEngine;

public static class PixelFieldMatrix
{
    static readonly Dictionary<int, Dictionary<int, Pixel>> Matrix = new Dictionary<int, Dictionary<int, Pixel>>();
    static readonly PixelPool Pool = new PixelPool();

    public static Pixel Show(int x, int y, Color color, bool canOverride = true)
    {
        if (Get(x, y, out var pixel))
        {
            if (canOverride)
                return pixel.SetColor(color);
            return pixel;
        }
        pixel = Pool.Get().Move(x, y).SetColor(color);
        Add(x, y, pixel);
        return pixel;
    }

    public static void Hide(int x, int y)
    {
        if (Get(x, y, out var pixel))
        {
            Pool.Return(pixel);
            Matrix[x].Remove(y);
        }
    }
    
    public static bool Add(int x, int y, Pixel pixel)
    {
        if (!Matrix.ContainsKey(x)) Matrix[x] = new Dictionary<int, Pixel>();
        if (Matrix.ContainsKey(x) && Matrix[x].ContainsKey(y)) return false;
        Matrix[x][y] = pixel;
        return true;
    }

    public static bool Get(int x, int y, out Pixel pixel)
    {
        if (Matrix.ContainsKey(x) && Matrix[x].ContainsKey(y))
        {
            pixel = Matrix[x][y];
            return true;
        }

        pixel = null;
        return false;
    }
}
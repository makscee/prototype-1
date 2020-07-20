using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils
{
    public static void CoordsFromDir(int dir, out int x, out int y)
    {
        switch (dir)
        {
            case 0:
                x = 0;
                y = 1;
                return;
            case 1:
                x = 1;
                y = 0;
                return;
            case 2:
                x = 0;
                y = -1;
                return;
            case 3:
                x = -1;
                y = 0;
                return;
            default:
                throw new Exception($"Wrong dir passed: {dir}");
        }
    }

    public static Vector2 CoordsFromDir(int dir)
    {
        CoordsFromDir(dir, out var x, out var y);
        return new Vector2(x, y);
    }

    public static int DirFromCoords(int x, int y)
    {
        if (x == 0 && y > 0) return 0;
        if (x > 0 && y == 0) return 1;
        if (x == 0 && y < 0) return 2;
        if (x < 0 && y == 0) return 3;
        throw new Exception($"Wrong coords for dir passed {x} {y}");
    }

    public static void Shuffle<T>(ref T[] a)
    {
        for (var i = 0; i < a.Length; i++)
        {
            var ind1 = Random.Range(0, a.Length);
            var ind2 = Random.Range(0, a.Length);
            var t = a[ind2];
            a[ind2] = a[ind1];
            a[ind1] = t;
        }
    }
    
    public static void GetInputCoords(out int x, out int y)
    {
        var worldPos = SharedObjects.Instance.Camera.ScreenToWorldPoint(Input.mousePosition);
        x = (int) Math.Round(worldPos.x);
        y = (int) Math.Round(worldPos.y);
    }
}
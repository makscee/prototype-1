using System;

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

    public static int DirFromCoords(int x, int y)
    {
        if (x == 0 && y > 0) return 0;
        if (x > 0 && y == 0) return 1;
        if (x == 0 && y < 0) return 2;
        if (x < 0 && y == 0) return 3;
        throw new Exception($"Wrong coords for dir passed {x} {y}");
    }
}
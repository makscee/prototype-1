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
}
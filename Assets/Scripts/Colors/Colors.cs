using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Colors
{
    static readonly Color[][] Palettes = {
        new[]
        {
            new Color(0.96f, 0.93f, 1f),
            new Color(0.86f, 0.84f, 0.97f),
            new Color(0.65f, 0.69f, 0.88f),
            new Color(0.26f, 0.28f, 0.45f),
        },
        new[]
        {
            new Color(0.49f, 0.35f, 0.35f),
            new Color(0.95f, 0.82f, 0.82f),
            new Color(0.95f, 0.88f, 0.88f),
            new Color(0.98f, 0.95f, 0.95f),
        },
        new[]
        {
            new Color(0.21f, 0.19f, 0.38f),
            new Color(0.3f, 0.3f, 0.49f),
            new Color(0.51f, 0.45f, 0.59f),
            new Color(0.85f, 0.73f, 0.76f),
        },
        new[]
        {
            new Color(0.66f, 0.9f, 0.81f),
            new Color(0.86f, 0.93f, 0.76f),
            new Color(1f, 0.83f, 0.71f),
            new Color(1f, 0.67f, 0.65f),
        },
    };

    static readonly Color[] SystemPalette =
        {
            new Color(0.98f, 0.96f, 0.91f),
            new Color(0.93f, 0.91f, 0.85f),
            new Color(1f, 0.99f, 0.96f),
            new Color(0.29f, 0.29f, 0.29f),
        };

    public static Color[] GetPalette(int id)
    {
        return id == -1 ? SystemPalette : Palettes[id];
    }

    public static int GetRandomFreeId()
    {
        var ids = Enumerable.Range(0, Palettes.Length).ToList();
        foreach (var palette in Roots.AllPalettes)
            if (palette != null)
                ids.Remove(palette.ColorsId);

        return ids[Random.Range(0, ids.Count)];
    }

    public static bool IsFreeId(int id)
    {
        return Roots.AllPalettes.All(palette => palette != null && palette.ColorsId != id);
    }
}
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


/*
 
new[]
{
    Color.black,
    Color.black,
    Color.black,
    Color.black
},

 */
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
            new Color(0.93f, 0.81f, 0.66f),
            new Color(0.91f, 0.62f, 0.44f),
            new Color(0.84f, 0.44f, 0.29f),
            new Color(0.67f, 0.29f, 0.19f)
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
            new Color(0.96f, 0.96f, 0.96f),
            new Color(0.94f, 0.65f, 0f),
            new Color(0.81f, 0.46f, 0f),
            new Color(0.1f, 0.11f, 0.13f),
        },
        new[]
        {
            new Color(0.95f, 0.95f, 0.97f),
            new Color(0.84f, 0.88f, 0.94f),
            new Color(0.55f, 0.58f, 0.67f),
            new Color(0.22f, 0.23f, 0.27f)
        },
        new[]
        {
            new Color(0.85f, 0.13f, 0.15f),
            new Color(1f, 0.57f, 0.2f),
            new Color(1f, 0.8f, 0.24f),
            new Color(0.21f, 0.82f, 0.73f)
        },
        new[]
        {
            new Color(0.44f, 0.29f, 0.56f),
            new Color(0.13f, 0.12f, 0.23f),
            new Color(0.02f, 0.02f, 0.02f),
            new Color(0.92f, 0.92f, 0.92f)
        },
        new[]
        {
            new Color(0.93f, 0.93f, 0.93f),
            new Color(0.2f, 0.88f, 0.77f),
            new Color(0.22f, 0.24f, 0.27f),
            new Color(0.13f, 0.16f, 0.19f)
        },
    };

    public static int Count => Palettes.Length;

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
        foreach (var palette in Roots.Palettes.Values)
            ids.Remove(palette.ColorsId);

        return ids[Random.Range(0, ids.Count)];
    }

    public static bool IsFreeId(int id)
    {
        return Roots.Palettes.Values.All(palette => palette != null && palette.ColorsId != id);
    }

    public static Action OnSomeRootPalettesChanged;
}
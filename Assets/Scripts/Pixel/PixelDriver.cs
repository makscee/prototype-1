using System.Collections.Generic;
using UnityEngine;

public static class PixelDriver
{
    static List<PixelRoad> _roads = new List<PixelRoad>();
    public static void Add(PixelRoad road)
    {
        _roads.Add(road);
    }

    public static Color GetColor(int x, int y)
    {
        var colors = new List<WeightedColor>();
        var totalWeight = 0f;
        foreach (var road in _roads)
        {
            var wc = road.GetColor(x, y);
            colors.Add(wc);
            totalWeight += wc.Weight;
        }

        var c = Color.clear;
        foreach (var weightedColor in colors)
        {
            c += weightedColor.Color * weightedColor.Weight / totalWeight;
        }
        return c;
    }

    public static void Update()
    {
        for (var i = 0; i < _roads.Count; i++)
        {
            _roads[i].Update();
            if (!_roads[i].IsDone) continue;
            _roads.RemoveAt(i);
            i--;
        }
    }

    static PixelDriver()
    {
        for (var i = 0; i < 30; i++)
            for (var j = 0; j < 30; j++)
                Pixel.Create(i - 15, j - 15);
    }
}
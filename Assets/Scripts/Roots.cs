using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Roots
{
    public static int Count => Blocks.Count(block => block != null);
    const int Limit = 4;
    public static readonly RootBlock[] Blocks = new RootBlock[Limit];
    
    // ReSharper disable InconsistentNaming
    static readonly GameObject[] _rootCanvases = new GameObject[Limit];
    static readonly Palette[] _palettes = new Palette[Limit];
    // ReSharper restore InconsistentNaming

    public static GameObject RootCanvases(int id)
    {
        if (_rootCanvases[id] == null)
            _rootCanvases[id] = GameObject.Find($"RootCanvas{id}");
        return _rootCanvases[id];
    }

    public static Palette Palettes(int id)
    {
        if (_palettes[id] == null)
            _palettes[id] = RootCanvases(id).GetComponent<Palette>();
        return _palettes[id];
    }

    public static IEnumerable<Palette> AllPalettes => _palettes.Where(p => p != null);
}
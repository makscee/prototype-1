using System;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Palette : MonoBehaviour
{
    public Color[] colors = new Color[4];
    public Color[] targetColors = new Color[4];
    public bool useOwnColors;
    public Color[] ownColors = new Color[4];

    public Palette copyOf;
    
    [SerializeField, Range(-1, 7)] int colorsId;

    public int ColorsId
    {
        get => This.colorsId;
        set
        {
            Colors.GetPalette(value).CopyTo(targetColors, 0);
            colorsId = value;
        }
    }

    const float LerpSpeed = 5f;
    void Update()
    {
        if (targetColors.SequenceEqual(colors)) return;
        // for (var i = 0; i < 4; i++)
        // {
        //     colors[i] = Color.Lerp(colors[i], targetColors[i], Time.deltaTime * LerpSpeed);
        // }
        targetColors.CopyTo(colors, 0);
    }

    void OnValidate()
    {
        PrepareColors();
    }


    void OnEnable()
    {
        PrepareColors();
    }

    void PrepareColors()
    {
        if (useOwnColors) ownColors.CopyTo(targetColors, 0);
        else Colors.GetPalette(ColorsId).CopyTo(targetColors, 0);
    }

    public Color GetColor(int paletteInd)
    {
        return copyOf != null ? copyOf.GetColor(paletteInd) : colors[paletteInd];
    }

    public void SetCopyOf(Palette palette) // for button calls
    {
        copyOf = palette;
    }

    Palette This => copyOf == null ? this : copyOf;
}
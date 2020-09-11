using UnityEngine;

[ExecuteInEditMode]
public class Palette : MonoBehaviour
{
    public Color[] colors = new Color[4];
    public bool useOwnColors;
    public Color[] ownColors = new Color[4];

    public Palette copyOf;
    
    [SerializeField, Range(-1, 7)] int colorsId;

    public int ColorsId
    {
        get => This.colorsId;
        set
        {
            Colors.GetPalette(value).CopyTo(colors, 0);
            colorsId = value;
        }
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
        if (useOwnColors) ownColors.CopyTo(colors, 0);
        else Colors.GetPalette(ColorsId).CopyTo(colors, 0);
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
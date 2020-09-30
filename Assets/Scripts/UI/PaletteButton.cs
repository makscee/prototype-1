using System;
using UnityEngine;
using UnityEngine.UI;

public class PaletteButton : MonoBehaviour
{
    [SerializeField] RawImage tint;
    public Palette palette;
    public PalettePicker palettePicker;

    [SerializeField] int id;
    void Start()
    {
        id = GetComponentInParent<RootIdHolder>().id;
        Colors.OnSomeRootPalettesChanged += Refresh;
        Refresh();
    }

    public void Apply()
    {
        if (!Available) return;
        Roots.Root[id].palette.ColorsId = palette.ColorsId;
        palettePicker.RefreshAll();
        Colors.OnSomeRootPalettesChanged();
    }

    bool Selected => Roots.Root[id].palette.ColorsId == palette.ColorsId;
    bool Available => Colors.IsFreeId(palette.ColorsId);

    public void Refresh()
    {
        transform.localScale = Selected
            ? new Vector3(1.2f, 1.2f)
            : !Selected && !Available ? new Vector3(0.6f, 0.6f) : Vector3.one;
        tint.color = !Selected && !Available ? new Color(0f, 0f, 0f, 0.32f) : Color.clear;
    }
}
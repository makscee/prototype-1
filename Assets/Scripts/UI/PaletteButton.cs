using System;
using UnityEngine;
using UnityEngine.UI;

public class PaletteButton : MonoBehaviour
{
    [SerializeField] PalettePicker palettePicker;
    [SerializeField] RawImage tint;
    [SerializeField] Palette palette;

    public void Apply()
    {
        if (!Available) return;
        Roots.RootCanvases(palettePicker.rootEditor.rootBlockId)
            .GetComponent<Palette>().copyOf = palette;
        palettePicker.RefreshAll();
    }

    void OnEnable()
    {
        Refresh();
    }

    bool Selected => Roots.Palettes(palettePicker.rootEditor.rootBlockId).ColorsId == palette.ColorsId;
    bool Available => Colors.IsFreeId(palette.ColorsId);

    public void Refresh()
    {
        transform.localScale = Selected ? new Vector3(1.2f, 1.2f, 1.2f) : Vector3.one;
        tint.color = !Selected && !Available ? new Color(0f, 0f, 0f, 0.32f) : Color.clear;
    }
}
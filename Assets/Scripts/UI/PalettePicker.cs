using UnityEngine;

public class PalettePicker : RootEditorScreen
{
    public PaletteButton[] paletteButtons;
    public override void Select()
    {
        gameObject.SetActive(true);
    }

    public override void Deselect()
    {
        gameObject.SetActive(false);
    }

    public void RefreshAll()
    {
        foreach (var palette in paletteButtons)
        {
            palette.Refresh();
        }
    }
}
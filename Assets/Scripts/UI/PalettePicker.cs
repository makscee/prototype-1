using System;
using System.Collections.Generic;
using UnityEngine;

public class PalettePicker : MonoBehaviour
{
    List<PaletteButton> _paletteButtons;
    [SerializeField] GameObject paletteButtonPrefab;

    void Start()
    {
        _paletteButtons = new List<PaletteButton>(GetComponentsInChildren<PaletteButton>());
        while (_paletteButtons.Count < Colors.Count)
        {
            _paletteButtons.Add(Instantiate(paletteButtonPrefab, transform).GetComponent<PaletteButton>());
        }
        for (var i = 0; i < _paletteButtons.Count; i++)
        {
            _paletteButtons[i].palettePicker = this;
            _paletteButtons[i].palette.ColorsId = i;
        }
    }

    public void RefreshAll()
    {
        foreach (var palette in _paletteButtons)
        {
            palette.Refresh();
        }
    }
}
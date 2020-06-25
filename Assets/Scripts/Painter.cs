using System;
using UnityEngine;
using UnityEngine.UI;

public class Painter : MonoBehaviour
{
    Action<Color> _paint;
    public Palette palette;
    [SerializeField]int numInPalette;
    public Painter subscribedTo;

    public int NumInPalette
    {
        get
        {
            if (subscribedTo == null) return numInPalette;
            return subscribedTo.NumInPalette;
        }
        set => numInPalette = value;
    }


    void Awake()
    {
        ObtainPaintAction();
    }

    void Update()
    {
        if (palette == null && subscribedTo == null) return;
        ColorRefresh();
    }

    Color _colorBefore;

    public Color Color
    {
        get
        {
            if (subscribedTo != null)
                return subscribedTo.Color;
            return palette.GetColor(numInPalette);
        }
    }

    void ColorRefresh()
    {
        if (_colorBefore == Color) return;
        _paint(Color);
        _colorBefore = Color;
    }

    void ObtainPaintAction()
    {
        var rawImg = GetComponent<RawImage>();
        if (rawImg != null)
        {
            _paint = c => rawImg.color = c;
            return;
        }

        var img = GetComponent<Image>();
        if (img != null)
        {
            _paint = c => img.color = c;
            return;
        }

        var text = GetComponent<Text>();
        if (text != null)
        {
            _paint = c => text.color = c;
            return;
        }

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            _paint = c => sr.color = c;
            return;
        }
        
        throw new Exception("No renderer found for Painter");
    }
}
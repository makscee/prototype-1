using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[ExecuteInEditMode]
public class Painter : MonoBehaviour
{
    Action<Color> _paint;
    public Palette palette;
    public Color Default;
    public bool ForceDefault;
    [SerializeField, Range(0, 3)]int numInPalette;
    public Painter subscribedTo;
    public Vector3 MultiplyBy = new Vector3(1, 1, 1);
    public Object PaintOn;

    public int NumInPalette
    {
        get
        {
            if (subscribedTo == null) return numInPalette;
            return subscribedTo.NumInPalette;
        }
        set => numInPalette = value;
    }

    void OnEnable()
    {
        if (palette == null) TryObtainPalette();
        ObtainPaintAction();
    }

    void Update()
    {
        PaintRefresh();
    }

    Color _colorBefore;

    public Color Color
    {
        get
        {
            if (subscribedTo != null)
                return subscribedTo.Color;
            if (palette == null) return Default;
            return palette.GetColor(numInPalette)
                   * new Color(MultiplyBy.x, MultiplyBy.y, MultiplyBy.z, 1);
        }
    }

    public void PaintRefresh()
    {
        if (_colorBefore == Color) return;
        _paint?.Invoke(Color);
        _colorBefore = Color;
    }

    void TryObtainPalette()
    {
        if (ForceDefault) return;
        palette = GetComponentInParent<Palette>();
    }

    void ObtainPaintAction()
    {
        
        var graphic = PaintOn == null ? GetComponent<Graphic>() : PaintOn as Graphic;
        if (graphic != null)
        {
            _paint = c =>
            {
                c.a = graphic.color.a;
                graphic.color = c;
            };
            return;
        }
        
        var rawImg = PaintOn == null ? GetComponent<RawImage>() : PaintOn as RawImage;
        if (rawImg != null)
        {
            _paint = c =>
            {
                c.a = rawImg.color.a;
                rawImg.color = c;
            };
            return;
        }

        var img = PaintOn == null ? GetComponent<Image>() : PaintOn as Image;
        if (img != null)
        {
            _paint = c => 
            {
                c.a = img.color.a;
                img.color = c;
            };
            return;
        }

        var text = PaintOn == null ? GetComponent<Text>() : PaintOn as Text;
        if (text != null)
        {
            _paint = c => 
            {
                c.a = text.color.a;
                text.color = c;
            };
            return;
        }

        var sr = PaintOn == null ? GetComponent<SpriteRenderer>() : PaintOn as SpriteRenderer;
        if (sr != null)
        {
            _paint = c => 
            {
                c.a = sr.color.a;
                sr.color = c;
            };
            return;
        }

        var outline = PaintOn == null ? GetComponent<Outline>() : PaintOn as Outline;
        if (outline != null)
        {
            _paint = c => 
            {
                c.a = outline.effectColor.a;
                outline.effectColor = c;
            };
            return;
        }

        var cam = PaintOn == null ? GetComponent<Camera>() : PaintOn as Camera;
        if (cam != null)
        {
            _paint = c => 
            {
                c.a = cam.backgroundColor.a;
                cam.backgroundColor = c;
            };
            return;
        }
        
        throw new Exception("No renderer found for Painter");
    }
}
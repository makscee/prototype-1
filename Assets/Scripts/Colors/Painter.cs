using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Painter : MonoBehaviour
{
    Action<Color> _paint;
    public Palette palette;
    [SerializeField, Range(0, 3)]int numInPalette;
    public Painter subscribedTo;
    public Vector3 MultiplyBy = new Vector3(1, 1, 1);

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
        if (Application.isPlaying)
            GameManager.InvokeAfterServiceObjectsInitialized(() => Gallery.Register(this));
    }

    void Update()
    {
        if (palette == null && subscribedTo == null) return;
        PaintRefresh();
    }

    Color _colorBefore;

    public Color Color
    {
        get
        {
            if (subscribedTo != null)
                return subscribedTo.Color;
            var galleryMultiplier = Gallery.Get(this);
            return palette.GetColor(numInPalette)
                   * new Color(MultiplyBy.x, MultiplyBy.y, MultiplyBy.z, 1)
                   * new Color(galleryMultiplier.x, galleryMultiplier.y, galleryMultiplier.z, 1);
        }
    }

    void PaintRefresh()
    {
        if (_colorBefore == Color) return;
        _paint?.Invoke(Color); 
        _colorBefore = Color;
    }

    void TryObtainPalette()
    {
        palette = GetComponentInParent<Palette>();
    }

    void ObtainPaintAction()
    {
        var rawImg = GetComponent<RawImage>();
        if (rawImg != null)
        {
            _paint = c =>
            {
                c.a = rawImg.color.a;
                rawImg.color = c;
            };
            return;
        }

        var img = GetComponent<Image>();
        if (img != null)
        {
            _paint = c => 
            {
                c.a = img.color.a;
                img.color = c;
            };
            return;
        }

        var text = GetComponent<Text>();
        if (text != null)
        {
            _paint = c => 
            {
                c.a = text.color.a;
                text.color = c;
            };
            return;
        }

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            _paint = c => 
            {
                c.a = sr.color.a;
                sr.color = c;
            };
            return;
        }

        var cam = GetComponent<Camera>();
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
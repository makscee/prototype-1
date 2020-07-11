using System;
using UnityEngine;
using UnityEngine.UI;

public class PostFxTexture
{
    static Texture2D _texture;
    static Color[] _pixels;

    public static Texture2D Texture
    {
        get
        {
            if (_texture == null)
            {
                _texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
                _pixels = new Color[_texture.width * _texture.height];
                Clear();
            }

            return _texture;
        }
    }

    public static void AddRect(Rect rect, Color color, bool apply = true)
    {
        var p1 = rect.position;
        var p2 = p1 + rect.size;
        var pxl1 = Camera.main.WorldToScreenPoint(p1);
        var pxl2 = Camera.main.WorldToScreenPoint(p2);

        for (var i = (int) pxl1.x; i <= pxl2.x && i >= 0 && i < Screen.width; i++)
        {
            for (var j = (int) pxl1.y; j <= pxl2.y && j >= 0 && j < Screen.height; j++)
            {
                _pixels[i + j * _texture.width] = color;
            }
        }

        if (apply)
        {
            _texture.SetPixels(_pixels);
            _texture.Apply();
        }
    }

    public static void Clear()
    {
        for (var i = 0; i < _pixels.Length; i++)
            _pixels[i] = Color.clear;
        AddRect(new Rect(Vector2.zero, new Vector2(100, 100)), Color.red, false);
        _texture.SetPixels(_pixels);
        _texture.Apply();
    }
}
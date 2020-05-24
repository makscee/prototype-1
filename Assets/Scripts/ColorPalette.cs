using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ColorPalette
{
    static readonly Color[][] Palettes = {
        new[]
        {
            new Color(0.96f, 0.93f, 1f),
            new Color(0.86f, 0.84f, 0.97f),
            new Color(0.65f, 0.69f, 0.88f),
            new Color(0.26f, 0.28f, 0.45f),
        },
        new[]
        {
            new Color(0.49f, 0.35f, 0.35f),
            new Color(0.95f, 0.82f, 0.82f),
            new Color(0.95f, 0.88f, 0.88f),
            new Color(0.98f, 0.95f, 0.95f),
        },
        new[]
        {
            new Color(0.21f, 0.19f, 0.38f),
            new Color(0.3f, 0.3f, 0.49f),
            new Color(0.51f, 0.45f, 0.59f),
            new Color(0.85f, 0.73f, 0.76f),
        },
        new[]
        {
            new Color(0.66f, 0.9f, 0.81f),
            new Color(0.86f, 0.93f, 0.76f),
            new Color(1f, 0.83f, 0.71f),
            new Color(1f, 0.67f, 0.65f),
        },
    };

    static int curPalette = 0;
    static Color[] _colors = new Color[4];

    static ColorPalette()
    {
        Palettes[0].CopyTo(_colors, 0);
    }

    static readonly Action<Color>[] _updateListeners = new Action<Color>[4];
    
    public static void SubscribeToPalette(Action<Color> a, int numInPalette)
    {
        _updateListeners[numInPalette] += a;
        a(_colors[numInPalette]);
    }

    public static void UnsubscribeFromPalette(Action<Color> a, int numInPalette)
    {
        _updateListeners[numInPalette] -= a;
    }

    static void Refresh()
    {
        for (var i = 0; i < 4; i++)
        {
            if (_updateListeners[i] == null) continue;
            _updateListeners[i](_colors[i]);
        }
    }

    public static void SwitchToPalette(int num)
    {
        _colors = Palettes[num];
        curPalette = num;
        Refresh();
    }

    public static void SwitchToNextPalette()
    {
        SwitchToPalette(++curPalette % Palettes.Length);
    }

    public static void AnimateSwitchToNextPalette()
    {
        _colors.CopyTo(_colorsFrom, 0);
        curPalette = (curPalette + 1) % Palettes.Length;
        Palettes[curPalette].CopyTo(_colorsTo, 0);
        t = 0f;
    }

    static Color[] _colorsFrom = new Color[4];
    static Color[] _colorsTo = new Color[4];
    static float _over = 1f, t = 1f;

    public static void Update()
    {
        if (t < _over)
        {
            t += Time.deltaTime;
            for (var i = 0; i < 4; i++)
            {
                _colors[i] = Color.Lerp(_colorsFrom[i], _colorsTo[i], t);
            }
            Refresh();
        }
    }

    public static void ClearListeners()
    {
        for (var i = 0; i < 4; i++)
        {
            _updateListeners[i] = null;
        }
    }

    public static void SubscribeGameObject(GameObject obj, int numInPalette)
    {
        Action<Color> a = null;
        var found = false;
        
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            found = true;
            a = color =>
            {
                if (sr == null)
                {
                    UnsubscribeFromPalette(a, numInPalette);
                    return;
                }
                sr.color = color;
            };
        }

        var rawImg = obj.GetComponent<RawImage>();
        if (rawImg != null)
        {
            found = true;
            a = color =>
            {
                if (rawImg == null)
                {
                    UnsubscribeFromPalette(a, numInPalette);
                    return;
                }
                rawImg.color = color;
            };
        }

        var img = obj.GetComponent<Image>();
        if (img != null)
        {
            found = true;
            a = color =>
            {
                if (img == null)
                {
                    UnsubscribeFromPalette(a, numInPalette);
                    return;
                }
                img.color = color;
            };
        }

        if (found)
        {
            SubscribeToPalette(a, numInPalette);
            return;
        }

        Debug.LogError($"Render component not found for {obj}");
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPalette
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

    int curPalette = 0;
    Color[] _colors = new Color[4];
    public ColorPalette(int num)
    {
        Palettes[num].CopyTo(_colors, 0);
    }

    void Refresh()
    {
        DoUnsubscribe(); // in case of unsub from outside
        foreach (var subscription in _subscribers.Values)
        {
            subscription.ApplyColor();
        }
        DoUnsubscribe();
    }

    public void SwitchToPalette(int num)
    {
        _colors = Palettes[num];
        curPalette = num;
    }

    public void AnimateSwitchToNextPalette()
    {
        _colors.CopyTo(_colorsFrom, 0);
        curPalette = (curPalette + 1) % Palettes.Length;
        Palettes[curPalette].CopyTo(_colorsTo, 0);
        t = 0f;
    }

    Color[] _colorsFrom = new Color[4];
    Color[] _colorsTo = new Color[4];
    float _over = 1f, t = 1f;

    public void Update()
    {
        if (t < _over)
        {
            t += Time.deltaTime;
            for (var i = 0; i < 4; i++)
            {
                _colors[i] = Color.Lerp(_colorsFrom[i], _colorsTo[i], t);
            }
        }
        Refresh();
    }

    class PaletteSubscription
    {
        public Action<Color> Action;
        public int NumInPalette;
        ColorPalette _palette;

        public PaletteSubscription(Action<Color> action, int numInPalette, ColorPalette palette)
        {
            Action = action;
            NumInPalette = numInPalette;
            _palette = palette;
            ApplyColor();
        }

        public void ApplyColor()
        {
            Action(_palette._colors[NumInPalette]);
        }
    }
    
    Dictionary<GameObject, PaletteSubscription> _subscribers = new Dictionary<GameObject, PaletteSubscription>();

    public void SubscribeToPalette(GameObject obj, Action<Color> action, int numInPalette)
    {
        _subscribers[obj] = new PaletteSubscription(action, numInPalette, this);
    }

    public void SubscribeToGameObject(GameObject to, GameObject self)
    {
        if (!_subscribers.ContainsKey(to)) Debug.LogError("Didn't find subscription object");
        var toSubscription = _subscribers[to];
        var subscription = new PaletteSubscription(GetColorAction(self), toSubscription.NumInPalette, this);
        subscription.Action = (color =>
        {
            if (!_subscribers.ContainsKey(to))
            {
                UnsubscribeFromPalette(self);
                return;
            }
            subscription.NumInPalette = _subscribers[to].NumInPalette;
        }) + subscription.Action;
        // subscription.ApplyColor();
        _subscribers[self] = subscription;
    }

    List<GameObject> _unsubscribeBuffer = new List<GameObject>();
    public void UnsubscribeFromPalette(GameObject obj)
    {
        _unsubscribeBuffer.Add(obj);
    }
    
    void DoUnsubscribe()
    {
        foreach (var obj in _unsubscribeBuffer)
        {
            _subscribers.Remove(obj);
        }
    }
    public void SubscribeGameObject(GameObject obj, int numInPalette)
    {
        if (_subscribers.ContainsKey(obj))
        {
            var subscription = _subscribers[obj];
            subscription.NumInPalette = numInPalette;
            // subscription.ApplyColor();
            return;
        }

        var a = GetColorAction(obj);
        var paletteSubscription = new PaletteSubscription(a, numInPalette, this);
        _subscribers[obj] = paletteSubscription;
        paletteSubscription.ApplyColor();
    }

    Action<Color> GetColorAction(GameObject obj)
    {
        if (obj == null) return color => { };
        
        Action<Color> a = null;
        
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            a = color =>
            {
                if (sr == null)
                {
                    UnsubscribeFromPalette(obj);
                    return;
                }
                color.a = sr.color.a;
                sr.color = color;
            };
        }

        var rawImg = obj.GetComponent<RawImage>();
        if (rawImg != null)
        {
            a = color =>
            {
                if (rawImg == null)
                {
                    UnsubscribeFromPalette(obj);
                    return;
                }
                color.a = rawImg.color.a;
                rawImg.color = color;
            };
        }

        var img = obj.GetComponent<Image>();
        if (img != null)
        {
            a = color =>
            {
                if (img == null)
                {
                    UnsubscribeFromPalette(obj);
                    return;
                }
                color.a = img.color.a;
                img.color = color;
            };
        }

        var text = obj.GetComponent<Text>();
        if (text != null)
        {
            a = color =>
            {
                if (text == null)
                {
                    UnsubscribeFromPalette(obj);
                    return;
                }
                color.a = text.color.a;
                text.color = color;
            };
        }

        if (a == null)
        {
            Debug.LogError($"Render component not found for {obj}");
        }

        return a;
    }
}
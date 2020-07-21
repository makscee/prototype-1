using System;
using System.Collections.Generic;
using UnityEngine;

public static class Gallery
{
    public enum CanvasMask
    {
        BackCanvas, MidCanvas, FrontCanvas, UiCanvas, Camera
    }

    public static readonly int LayersCount = Enum.GetValues(typeof(CanvasMask)).Length;

    static HashSet<Painter>[] _painters = new HashSet<Painter>[LayersCount];
    public static Vector3[] Multipliers;

    static Gallery()
    {
        Multipliers = new Vector3[_painters.Length];
        for (var i = 0; i < _painters.Length; i++)
        {
            _painters[i] = new HashSet<Painter>();
            Multipliers[i] = new Vector3(1, 1, 1);
        }
    }

    public static void Register(Painter painter)
    {
        if (painter == null) return;
        var canvas = painter.GetComponentInParent<Canvas>()?.gameObject;
        if (canvas == null)
        {
            if (painter.GetComponent<Camera>())
            {
                _painters[(int) CanvasMask.Camera].Add(painter);
                return;
            }
            Debug.LogError(painter.gameObject);
            return;
        }

        if (canvas == SharedObjects.Instance.BackCanvas)
        {
            _painters[(int) CanvasMask.BackCanvas].Add(painter);
            return;
        }
        if (canvas == SharedObjects.Instance.MidCanvas)
        {
            _painters[(int) CanvasMask.MidCanvas].Add(painter);
            return;
        }
        if (canvas == SharedObjects.Instance.FrontCanvas)
        {
            _painters[(int) CanvasMask.FrontCanvas].Add(painter);
            return;
        }
        if (canvas == SharedObjects.Instance.UICanvas)
        {
            _painters[(int) CanvasMask.UiCanvas].Add(painter);
            return;
        }

        Debug.LogError($"no canvas found for {painter.gameObject}. canvas: {canvas}");
    }

    public static Vector3 Get(Painter painter)
    {
        for (var i = 0; i < _painters.Length; i++)
            if (_painters[i].Contains(painter))
            {
                return Multipliers[i];
            }
        return new Vector3(1, 1, 1);
    }

    public static void AddByMask(float r, float g, float b, int mask)
    {
        for (var i = 0; i < _painters.Length; i++)
        {
            if ((1 << i & mask) > 0)
            {
                Multipliers[i] += new Vector3(r, g, b);
            }
        }
    }

    public static void AddByMask(float value, int mask)
    {
        AddByMask(value, value, value, mask);
    }
}
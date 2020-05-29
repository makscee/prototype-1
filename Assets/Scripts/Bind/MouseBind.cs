using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MouseBind : IBindable
{
    Bind _bind;
    MouseBind()
    {
    }

    public Vector2 GetPosition()
    {
        var v = SharedObjects.Instance.Camera.ScreenToWorldPoint(Input.mousePosition);
        return v;
    }

    public bool IsAnchor()
    {
        return true;
    }

    public bool IsAnchored()
    {
        return true;
    }

    public void SetAnchored(bool value)
    {
    }

    public bool Used { get; set; }

    static MouseBind _instance;
    public static MouseBind Get()
    {
        return _instance ?? (_instance = new MouseBind());
    }
}
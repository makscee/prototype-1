using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MouseCreateBind : IBindable
{
    Bind _bind;
    MouseCreateBind()
    {
    }

    public Vector2 GetPosition()
    {
        var v = Input.mousePosition;
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

    static MouseCreateBind _instance;
    public static MouseCreateBind Get()
    {
        return _instance ?? (_instance = new MouseCreateBind());
    }
}
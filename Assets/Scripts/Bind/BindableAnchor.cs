using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BindableAnchor : IBindable
{
    public abstract Vector2 GetPosition();
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
}
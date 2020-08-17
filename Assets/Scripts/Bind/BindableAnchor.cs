using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BindableAnchor : IBindable
{
    public abstract Vector2 GetPosition();
    public virtual bool IsAnchor()
    {
        return true;
    }

    public bool IsAnchored
    {
        get => true;
        set { }
    }

    public bool Used { get; set; }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBindable
{
    Vector2 GetPosition();

    bool IsAnchor();

    bool IsAnchored { get; set; }
    
    bool Used { get; set; }
}
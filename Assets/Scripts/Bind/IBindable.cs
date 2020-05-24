using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBindable
{
    Vector2 GetPosition();

    bool IsAnchor();

    bool IsAnchored();

    void SetAnchored(bool value);
    
    bool Used { get; set; }
}
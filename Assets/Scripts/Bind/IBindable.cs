using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBindable
{
    Vector2 GetPosition();
    
    bool Used { get; set; }
}
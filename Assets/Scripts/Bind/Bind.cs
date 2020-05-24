using System;
using System.Collections.Generic;
using UnityEngine;

public class Bind
{
    public Bind(IBindable fist, IBindable second, Vector2 offset, int strength, float breakDistance = -1)
    {
        First = fist;
        Second = second;
        Offset = offset;
        Strength = strength;
        BreakDistance = breakDistance;
        BindVisual.Create(fist, second, breakDistance);
    }
    
    public IBindable First, Second;
    
    // from first to second
    public readonly Vector2 Offset;

    public readonly int Strength;
    public readonly float BreakDistance;
    public const int StrengthMultiplier = 5;

    public bool Used(IBindable obj)
    {
        return obj == First || obj == Second;
    }

    public Vector2 GetTarget(IBindable self)
    {
        if (First != self && Second != self) throw new Exception("Trying to get target for object that is not part of the bind");

        Vector2 target;
        if (First == self) target = Second.GetPosition() - Offset;
        else target = First.GetPosition() + Offset;
        return target;
    }

    // return true if was broken
    public void Update()
    {
        Debug.DrawLine(First.GetPosition(), Second.GetPosition(), Color.green);

        if (BreakDistance > -1 && (First.GetPosition() - Second.GetPosition()).magnitude > BreakDistance)
        {
            Break();
        }
    }

    public void Break()
    {
        BindMatrix.RemoveBind(First, Second);
    }
}
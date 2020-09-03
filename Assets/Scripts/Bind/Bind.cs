using System;
using System.Collections.Generic;
using UnityEngine;

public class Bind
{
    public const int BlockBindStrength = 5;
    public const int BlockStaticBindStrength = 10;
    public const int MouseBindStrength = 8;
    public const int PulseBlockBindStrength = 14;
    
    public Bind(IBindable fist, IBindable second, Vector2 offset, int strength, float ropeLength = 0, float breakDistance = -1)
    {
        First = fist;
        Second = second;
        Offset = offset;
        Strength = strength;
        RopeLength = ropeLength;
        BreakDistance = breakDistance;
    }

    public readonly IBindable First, Second;
    
    // first -> second
    public readonly Vector2 Offset;

    public readonly int Strength;
    public readonly float BreakDistance;
    public readonly float RopeLength;
    public const int StrengthMultiplier = 3;

    public bool Used(IBindable obj)
    {
        return obj == First || obj == Second;
    }

    public Vector2 GetTarget(IBindable self)
    {
        if (First != self && Second != self) throw new Exception("Trying to get target for object that is not part of the bind");
        Vector2 target;
        var firstPos = First.GetPosition();
        var secondPos = Second.GetPosition();
        var selfPos = self.GetPosition();
        if (First == self) target = secondPos - Offset;
        else target = firstPos + Offset;

        if (RopeLength > 0)
        {
            if (IsLoose()) return selfPos;
            
            var dist = (selfPos - target).magnitude;
            return (target - selfPos).normalized * (dist - RopeLength) + selfPos;
        }
        return target;
    }
    
    public void Update()
    {
        Debug.DrawLine(First.GetPosition(), Second.GetPosition(), Color.green);

        if (BreakDistance > -1 && (First.GetPosition() - Second.GetPosition()).magnitude > BreakDistance)
        {
            Break();
        }
    }

    public bool IsLoose()
    {
        return RopeLength > 0f && (First.GetPosition() + Offset - Second.GetPosition()).magnitude < RopeLength;
    }

    public void Break()
    {
        BindMatrix.RemoveBind(First, Second);
    }
}
using System;
using UnityEngine;

public class BlockPhysics : MonoBehaviour
{
    public float X => parent.transform.position.x;
    public float Y => parent.transform.position.y;

    public Vector2 Position => parent.transform.position;
    
    public Block parent;

    Vector2 _velocity;
    Vector2 _desiredVelocity;
    const float MaxAcceleration = 900;

    void Update()
    {
        var maxSpeedChange = MaxAcceleration * Time.deltaTime;
        // _velocity = Vector2.ClampMagnitude(parent.logic.Position - Position, 50f);
        var bindStrengthDivisor = 1f / BindMatrix.GetBindsCount(parent);
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(parent))
        {
            var v = bind.GetTarget(parent) - Position;
            var f = v * (bind.Strength * bindStrengthDivisor * Bind.StrengthMultiplier);

            _desiredVelocity += f;
        }
        _velocity = Vector2.MoveTowards(_velocity, _desiredVelocity, maxSpeedChange);
        parent.transform.position += (Vector3)_velocity * Time.deltaTime;
        _desiredVelocity = Vector2.zero;
    }
}
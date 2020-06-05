using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BindableMonoBehavior : MonoBehaviour, IBindable
{
    public bool Movable;
    
    float _speedRand;

    protected virtual void Start()
    {
        _speedRand = 1f; //Random.Range(1f, 1.1f);
    }

    protected Vector2 Velocity;
    protected virtual void FixedUpdate()
    {
        if (!Movable) return;
        var force = Vector2.zero;
        
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
        {
            var v = bind.GetTarget(this) - GetPosition();
            var f = v * (bind.Strength * Bind.StrengthMultiplier);

            force += f;
        }
        if (!IsAnchored())
        {
            const float radius = 0.1f; 
            const float speed = 1f;
            var t = Time.time;
            force += new Vector2(Mathf.Cos(t * speed * _speedRand) * radius, Mathf.Sin(t * speed * _speedRand) * radius);
        }

        Velocity += (force - Velocity) * 0.8f;
        transform.position += (Vector3)Velocity * (Time.fixedDeltaTime);
    }

    public Vector2 GetPosition()
    {
        if (!_destroyed)
            return transform.position;
        return Vector2.zero;
    }

    public virtual bool IsAnchor()
    {
        return false;
    }

    bool _anchored;
    public virtual bool IsAnchored()
    {
        return _anchored;
    }

    public virtual void SetAnchored(bool value)
    {
        _anchored = value;
    }

    public bool Used { get; set; }

    bool _destroyed;
    protected virtual void OnDestroy()
    {
        _destroyed = true;
    }
}
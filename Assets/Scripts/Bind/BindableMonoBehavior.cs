using UnityEngine;

public class BindableMonoBehavior : MonoBehaviour, IBindable
{
    public bool Movable;

    protected Vector2 Velocity;
    public Vector2 DesiredVelocity;
    public const float MaxAcceleration = 300;
    protected virtual void Update()
    {
        if (!Movable) return;
        var maxSpeedChange = MaxAcceleration * Time.deltaTime;
        
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
        {
            var v = bind.GetTarget(this) - GetPosition();
            var f = v * (bind.Strength * Bind.StrengthMultiplier);

            DesiredVelocity += f;
        }
        if (!IsAnchored())
        {
            const float radius = 0.1f;
            const float speed = 1f;
            var t = Time.time;
            DesiredVelocity += new Vector2(Mathf.Cos(t * speed) * radius, Mathf.Sin(t * speed) * radius);
        }
        Velocity = Vector2.MoveTowards(Velocity, DesiredVelocity, maxSpeedChange);
        transform.position += (Vector3)Velocity * (Time.deltaTime);
        DesiredVelocity = Vector2.zero;
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
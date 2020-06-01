using UnityEngine;
using Random = UnityEngine.Random;

public class BindableMonoBehavior : MonoBehaviour, IBindable
{
    public bool Movable;
    
    float _speedRand;

    protected virtual void Start()
    {
        _speedRand = Random.Range(1f, 1.1f);
    }

    Vector2 _velocity;
    protected virtual void FixedUpdate()
    {
        if (!Movable) return;
        var force = Vector2.zero;
        
        if (IsAnchored())
        {
            foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
            {
                var v = bind.GetTarget(this) - GetPosition();
                var f = v * (bind.Strength * Bind.StrengthMultiplier);

                force += f;
            }

        }
        else
        {
            const float radius = 0.5f; 
            const float speed = 4f;
            var t = Time.time;
            force += new Vector2(Mathf.Cos(t * speed * _speedRand) * radius, Mathf.Sin(t * speed * _speedRand) * radius);
        }

        _velocity += (force - _velocity) / 2;
        transform.position += (Vector3)_velocity * (Time.fixedDeltaTime);
    }

    public Vector2 GetPosition()
    {
        return transform.position;
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

    public void SetAnchored(bool value)
    {
        _anchored = value;
    }

    public bool Used { get; set; }
}
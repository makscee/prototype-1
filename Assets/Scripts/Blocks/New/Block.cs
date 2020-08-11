using System;
using UnityEngine;

public class Block : MonoBehaviour, IBindable, IBindHandler
{
    public BlockLogic logic;
    public BlockView view;
    public BlockPhysics physics;
    [HideInInspector]
    public int rootDirection;

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    protected bool AnchorSelf;
    public bool IsAnchor()
    {
        return AnchorSelf;
    }

    public bool IsAnchored { get; set; }

    public bool Used { get; set; }
    public void OnBind(Bind bind)
    {
        logic.OnBind(bind);
        view.OnBind(bind);
    }

    public void OnUnbind(Bind bind)
    {
        logic.OnUnbind(bind);
        view.OnUnbind(bind);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour, IBindable, IBindHandler
{
    public BlockLogic logic;
    public BlockView view;
    public BlockPhysics physics;
    [HideInInspector]
    public int rootDirection;

    protected virtual void OnEnable()
    {
        logic.onBeginDrag += eventData =>
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                BindMatrix.AddBind(this, MouseBind.Get(), Vector2.zero, Bind.MouseBindStrength);
        };
        logic.onEndDrag += eventData =>
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                BindMatrix.RemoveBind(this, MouseBind.Get());
        };
    }

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

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour, IBindable, IBindHandler
{
    public BlockLogic logic;
    public BlockView view;
    public BlockPhysics physics;
    public int rootId;
    public bool PulseConnected => Roots.Blocks[rootId].pulseVersion == pulseVersion;
    public int pulseVersion;

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

    protected void Start()
    {
        if (!_inited) Debug.LogError($"StartInit wasn't called {name}");
    }

    bool _inited;
    protected virtual void StartInit()
    {
        _inited = true;
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

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
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockLogic : MonoBehaviour
{
    public Vector2 Position => new Vector2(X, Y);

    public bool HasPulse { get; private set; }
    public int stepNumber = int.MaxValue / 2;


    [SerializeField]int _x, _y;
    public int X
    {
        get => _x;
        private set => _x = value;
    }

    public int Y
    {
        get => _y;
        private set => _y = value;
    }

    public void SetCoords(int x, int y)
    {
        FieldMatrix.ClearMe(parent);
        FieldMatrix.Add(x, y, parent);
        X = x;
        Y = y;
    }

    public Block parent;
    [NonSerialized] public Action<PointerEventData> onTap;
    [NonSerialized] public Action<Block> onPulseReceive;
    [NonSerialized] public Action<PointerEventData> onBeginDrag, onDrag, onEndDrag;
    [NonSerialized] public Action<Bind> onBind, onUnbind;

    void OnEnable()
    {
        FieldMatrix.Add(X, Y, parent);
    }

    void OnDisable()
    {
        GlobalPulse.UnsubscribeFromNext(PassPulse);
        BindMatrix.RemoveAllBinds(parent);
        FieldMatrix.ClearMe(parent);
    }

    void Update()
    {
        // UpdateUnanchoredCoords();
    }

    void UpdateUnanchoredCoords()
    {
        // if (parent.IsAnchored) return;
        
        var x = Mathf.RoundToInt(parent.physics.X);
        var y = Mathf.RoundToInt(parent.physics.Y);
        if (x != X || y != Y)
            SetCoords(x, y);
    }

    public void ReceivePulse(Block from = null)
    {
        if (HasPulse) return;
        onPulseReceive?.Invoke(from);
        GlobalPulse.SubscribeToNext(PassPulse);
        HasPulse = true;
        parent.view.SetDirty();
    }

    void PassPulse()
    {
        if (BindMatrix.GetOutBindsCount(parent) > 0)
        {
            foreach (var bind in BindMatrix.GetAllAdjacentBinds(parent))
            {
                if (bind.First == parent && bind.Second is Block block)
                {
                    block.logic.ReceivePulse(parent);
                }
            }
        }

        HasPulse = false;
        parent.view.SetDirty();
    }

    public void OnBind(Bind bind)
    {
        onBind?.Invoke(bind);
    }
    
    public void OnUnbind(Bind bind)
    {
        onUnbind?.Invoke(bind);
    }

    public void Click(PointerEventData eventData)
    {
        if (_dragging) return;
        onTap?.Invoke(eventData);
    }

    public void Drag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData);
    }

    bool _dragging;

    public void BeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        onBeginDrag?.Invoke(eventData);
    }

    public void EndDrag(PointerEventData eventData)
    {
        _dragging = false;
        onEndDrag?.Invoke(eventData);
    }
}
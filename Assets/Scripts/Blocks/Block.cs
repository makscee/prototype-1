using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Block : BindableMonoBehavior, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBindHandler
{
    [SerializeField]
    int _x, _y;

    public const float BlockSide = 1;

    public Text text;
    public PulseBlock pulseBlock;
    public Painter painter, insidePainter;

    public Action onTap;

    bool _initialized;
    protected virtual void OnEnable()
    {
        if (_initialized) return;
        onTap = () => BlockEditor.OnBlockClick(this);
        Init();
        _initialized = true;
    }

    protected virtual void Init()
    {
        
    }


    public int X => _x;
    public int Y => _y;
    void SetCoords(int x, int y)
    {
        if (x != _x || y != _y)
        {
            if (FieldMatrix.Get(_x, _y, out var block) && block == this)
                FieldMatrix.Clear(_x, _y);
        }
        _x = x;
        _y = y;
        FieldMatrix.Add(x, y, this);
    }

    void SetInsideCheckerboardColor()
    {
        var xt = _x + 1000000;
        var yt = _y + 1000000;
        insidePainter.NumInPalette = 1 - (xt + yt) % 2;
    }

    public static Block Create()
    {
        var block = Instantiate(Prefabs.Instance.Block, SharedObjects.Instance.FrontCanvas.transform).GetComponent<Block>();
        return block;
    }

    public static Block Create(Block parent, int x, int y)
    {
        var b = Create();
        var newBlockOffset = new Vector2(x - parent.X, y - parent.Y);
        b.transform.position = parent.transform.position + (Vector3)newBlockOffset * .8f;
        b.SetCoords(x, y);
        b.SetInsideCheckerboardColor();
        FieldMatrix.Add(x, y, b);
        b.pulseBlock = parent.pulseBlock;
        var p = b.pulseBlock.GetComponent<Palette>();
        b.GetComponent<Painter>().palette = p;
        foreach (var painter in b.GetComponentsInChildren<Painter>())
        {
            painter.palette = p;
        }
        BindMatrix.AddBind(parent, b, newBlockOffset, Bind.BlockBindStrength);
        return b;
    }

    public static Block Create(int x, int y, int pulseBlockX, int pulseBlockY)
    {
        var b = Create();
        if (!FieldMatrix.Get(pulseBlockX, pulseBlockY, out var pulseBlock))
        {
            Debug.LogError($"No pulse block {pulseBlockX} {pulseBlockY}");  
            return b;
        }
        b.transform.position = new Vector3(x, y, 0f);
        b.SetCoords(x, y);
        b.pulseBlock = (PulseBlock) pulseBlock;
        var p = pulseBlock.GetComponent<Palette>();
        b.GetComponent<Painter>().palette = p;
        foreach (var painter in b.GetComponentsInChildren<Painter>())
        {
            painter.palette = p;
        }
        b.SetInsideCheckerboardColor();
        return b;
    }

    protected override void Update()
    {
        if (!IsAnchored())
        {
            if (BindMatrix.GetBindsCount(this) == 0)
            {
                Destroy();
            }
            UpdateCoordsFromTransformPosition();
        }
        base.Update();
    }

    protected void UpdateCoordsFromTransformPosition()
    {
        var pos = transform.position;
        SetCoords((int)Math.Round(pos.x), (int)Math.Round(pos.y));
    }

    bool _dragging, _placeholdersShown;
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!masked)
                BindMatrix.AddBind(this, MouseBind.Get(), Vector2.zero, Bind.MouseBindStrength);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        if (cancelDrag)
        {
            cancelDrag = false;
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // TryCreateBlock();
            if (!masked)
                BindMatrix.RemoveBind(this, MouseBind.Get());
        }
    }

    protected bool cancelDrag;
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount > 1)
        {
            CancelDrag();
        }
    }

    public virtual void CancelDrag()
    {
        cancelDrag = true;
        if (!masked)
            BindMatrix.RemoveBind(this, MouseBind.Get());
    }

    IEnumerable<Block> CollectBoundBlocks()
    {
        var result = new List<Block>();
        foreach (var obj in BindMatrix.CollectBoundCluster(this))
        {
            if (obj is Block block) result.Add(block);
        }

        return result;
    }

    readonly List<Block> _lastPulseFrom = new List<Block>();
    public virtual void ReceivePulse(Block from)
    {
        _lastPulseFrom.Add(from);
        insidePainter.NumInPalette = 3;
        GlobalPulse.SubscribeToNext(PassPulse);
    }

    public virtual void PassPulse()
    {
        var passed = false;
        SetInsideCheckerboardColor();
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
        {
            if (bind.First == this && bind.Second is Block block)
            {
                block.ReceivePulse(this);
                passed = true;
            }
        }

        if (!passed)
        {
            foreach (var last in _lastPulseFrom) // todo stop pulse propagation on 2 pulse sources
            {
                var v = transform.position - last.transform.position;
                var x = X - last.X;
                var y = Y - last.Y;
                transform.position += v;
                if (x > 0)
                    pulseBlock.OnPulseDeadEnd(1);
                else if (x < 0)
                    pulseBlock.OnPulseDeadEnd(3);
                else if (y > 0)
                    pulseBlock.OnPulseDeadEnd(0);
                else if (y < 0)
                    pulseBlock.OnPulseDeadEnd(2);
            }
        }
        _lastPulseFrom.Clear();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging || Input.touchCount > 1) return;
        Debug.Log($"pointer click {Input.touchCount}");
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            OnMiddleClick();
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    protected virtual void OnMiddleClick()
    {
        Destroy();
    }

    protected virtual void OnLeftClick()
    {
        onTap?.Invoke();
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        return;
        var away = (Vector2)(transform.position - other.transform.position);
        if (other is CircleCollider2D circle)
        {
            DesiredVelocity +=  (circle.radius * 2 - away.magnitude) * 120 * away.normalized;
        }
    }

    public void Destroy()
    {
        BindMatrix.RemoveAllBinds(this);
        Destroy(gameObject);
    }
    public void DisplayText(string s)
    {
        if (s.Length > 3) s = "";
        if (text)
            text.text = s;
    }

    public bool masked;
    public void SetMasked(bool value)
    {
        masked = value;
        if (value)
        {
            PixelFieldMatrix.Show(X, Y, Color.red).SetShadow(true);
            transform.position = new Vector3(X, Y);
            RefreshMaskSqueeze();
            gameObject.SetActive(false);
        }
        else
        {
            PixelFieldMatrix.Hide(X, Y);
            gameObject.SetActive(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    [SerializeField]
    int stepNumber = int.MaxValue / 2;
    protected int StepNumber
    {
        get => stepNumber;
        set
        {
            stepNumber = value;
            DisplayText(stepNumber.ToString());
        }
    }

    FieldCircle _fieldCircle;
    protected virtual void RefreshFieldCircle()
    {
        var show = true;
        if (!IsAnchored())
        {
            show = false;
        }
        else
        {
            foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
            {
                if (bind.First == this && bind.Second is Block)
                {
                    show = false;
                    break;
                }
            }
        }

        if (show && _fieldCircle == null && SharedObjects.Instance.MidCanvas != null)
        {
            _fieldCircle = FieldCircle.Create(transform);
            var p = _fieldCircle.GetComponent<Painter>();
            p.NumInPalette = 1;
            p.palette = pulseBlock.palette;
        } else if (!show && _fieldCircle != null)
        {
            Destroy(_fieldCircle.gameObject);
            _fieldCircle = null;
        }
    }

    public void OnBind(Bind bind)
    {
        if (bind.Second == this && bind.First is Block)
        {
            RefreshStepNumber();
        }

        RefreshMaskSqueeze();
        RefreshFieldCircle();
    }

    public void OnUnbind(Bind b)
    {
        if (b.Second == this && b.First is Block) RefreshStepNumber();
        RefreshFieldCircle();
        RefreshMaskSqueeze();
    }

    protected virtual void RefreshMaskSqueeze()
    {
        if (!masked) return;
        if (!PixelFieldMatrix.Get(X, Y, out var pixel)) return;
        var binds = BindMatrix.GetAllAdjacentBinds(this).ToArray();
        if (binds.Length != 2)
        {
            pixel.ResetScale();
            return;
        }
        if (binds[0].First != binds[1].First &&
            binds[0].Second != binds[1].Second &&
            binds[0].Offset == binds[1].Offset)
        {
            pixel.Squeeze(binds[0].Offset.x == 0f);
        }
    }

    public virtual void RefreshStepNumber()
    {
        var t = int.MaxValue / 2;
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
        {
            if (bind.Second == this && bind.First is Block block)
            {
                t = Math.Min(t, block.StepNumber + 1);
            }
        }

        if (t == StepNumber) return;
        StepNumber = t;
        StepNumberChangeNotify();
    }

    public void StepNumberChangeNotify()
    {
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
        {
            if (bind.First == this && bind.Second is Block block)
            {
                block.RefreshStepNumber();
            }
        }
    }

    protected override void OnDestroy()
    {
        FieldMatrix.ClearMeDaddy(this);
        GlobalPulse.UnsubscribeFromNext(PassPulse);
        base.OnDestroy();
    }
}

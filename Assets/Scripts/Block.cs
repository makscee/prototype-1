using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Block : BindableMonoBehavior, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBindHandler
{
    [SerializeField]
    int _x, _y;
    public GameObject inside;
    public GameObject text;

    public const float BlockSide = 1;

    public PulseBlock PulseBlock;

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

    void RevertToDefaultColor()
    {
        var xt = _x + 1000000;
        var yt = _y + 1000000;
        PulseBlock.ColorPalette.SubscribeGameObject(inside, 1 - (xt + yt) % 2);
    }

    void OnEnable()
    {
        _text = text.GetComponent<Text>();
    }

    protected virtual void Start()
    {
        PulseBlock.ColorPalette.SubscribeGameObject(gameObject, 3);
        PulseBlock.ColorPalette.SubscribeGameObject(text, 3);
        RevertToDefaultColor();
    }

    public static Block Create()
    {
        return Instantiate(Prefabs.Instance.Block, SharedObjects.Instance.FrontCanvas.transform).GetComponent<Block>();
    }

    public static Block Create(Block parent, int x, int y)
    {
        var b = Create();
        b.PulseBlock = parent.PulseBlock;
        b.transform.position = parent.transform.position;
        b.SetCoords(x, y);
        b.RevertToDefaultColor();
        FieldMatrix.Add(x, y, b);
        var newBlockOffset = new Vector2(x - parent.X, y - parent.Y);
        BindMatrix.AddBind(parent, b, newBlockOffset, Bind.BlockBindStrength);
        return b;
    }

    public static Block Create(int x, int y, int pulseBlockX, int pulseBlockY)
    {
        var b = Create();
        FieldMatrix.Get(pulseBlockX, pulseBlockY, out var pulseBlock);
        b.PulseBlock = (PulseBlock) pulseBlock;
        b.transform.position = new Vector3(x, y, 0f);
        b.SetCoords(x, y);
        b.RevertToDefaultColor();
        return b;
    }

    protected override void Update()
    {
        if (!IsAnchored())
        {
            UpdateCoordsFromTransformPosition();
        }
        base.Update();
        TryDrawPlaceholdersOnDrag();
    }

    protected void UpdateCoordsFromTransformPosition()
    {
        var pos = transform.position;
        SetCoords((int)Math.Round(pos.x), (int)Math.Round(pos.y));
    }

    bool _dragging, _placeholdersShown;
    void TryDrawPlaceholdersOnDrag()
    {
        if (!_dragging)
        {
            ShowNewBlockPlaceholders(false);
            return;
        }
        var bind = BindMatrix.GetBind(this, MouseBind.Get());
        if (bind == null)
        {
            ShowNewBlockPlaceholders(false);
            return;
        }
        if (bind.IsLoose()) ShowNewBlockPlaceholders(true);
        else ShowNewBlockPlaceholders(false);
    }
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            BindMatrix.AddBind(this, MouseBind.Get(), Vector2.zero, Bind.MouseBindStrength, 1.5f);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            TryCreateBlock();
            BindMatrix.RemoveBind(this, MouseBind.Get());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    // true if something affected
    protected virtual bool TryCreateBlock()
    {
        var selfPos = GetPosition();
        var pos = MouseBind.Get().GetPosition();
        var size = new Vector2(BlockSide, BlockSide);
        var up = new Rect(selfPos + new Vector2(0, BlockSide) - size / 2, size);
        var down = new Rect(selfPos + new Vector2(0, -BlockSide) - size / 2, size);
        var left = new Rect(selfPos + new Vector2(-BlockSide, 0) - size / 2, size);
        var right = new Rect(selfPos+ new Vector2(BlockSide, 0) - size / 2, size);

        int x, y;
        if (up.Contains(pos))
        {
            x = X;
            y = Y + 1;
        }
        else if (down.Contains(pos))
        {
            x = X;
            y = Y - 1;
        }
        else if (left.Contains(pos))
        {
            x = X - 1;
            y = Y;
        }
        else if (right.Contains(pos))
        {
            x = X + 1;
            y = Y;
        }
        else
        {
            BindMatrix.RemoveBind(this, MouseBind.Get());
            return false;
        }
        var newBlockOffset = new Vector2(x - X, y - Y);
 
        if (FieldMatrix.Get(x, y, out var existingBlock))
        {
            var bind = BindMatrix.GetBind(this, existingBlock);
            if (bind == null)
                BindMatrix.AddBind(this, existingBlock, newBlockOffset, Bind.BlockBindStrength);
            else if (bind.First != this)
            {
                bind.Break();
                BindMatrix.AddBind(this, existingBlock, newBlockOffset, Bind.BlockBindStrength);
            } else if (bind.First == this)
            {
                bind.Break();
            }
            return true;
        }
        var b = Create(this, x, y);
        return true;
    }

    IEnumerable<Block> CollectBoundBlocks()
    {
        var result = new List<Block>();
        foreach (var obj in BindMatrix.CollectAllBoundObjects(this))
        {
            if (obj is Block block) result.Add(block);
        }

        return result;
    }

    readonly List<Block> _lastPulseFrom = new List<Block>();
    public virtual void ReceivePulse(Block from)
    {
        _lastPulseFrom.Add(from);
        PulseBlock.ColorPalette.SubscribeGameObject(inside, 3);
        GlobalPulse.SubscribeToNext(PassPulse);
    }

    public virtual void PassPulse()
    {
        var passed = false;
        RevertToDefaultColor();
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
                    PulseBlock.OnPulseDeadEnd(1);
                else if (x < 0)
                    PulseBlock.OnPulseDeadEnd(3);
                else if (y > 0)
                    PulseBlock.OnPulseDeadEnd(0);
                else if (y < 0)
                    PulseBlock.OnPulseDeadEnd(2);
            }
        }
        _lastPulseFrom.Clear();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
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
        
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        var away = (Vector2)(transform.position - other.transform.position);
        if (other is CircleCollider2D circle)
        {
            DesiredVelocity +=  (circle.radius * 2 - away.magnitude) * 120 * away.normalized;
        }
    }

    protected virtual void ShowNewBlockPlaceholders(bool value)
    {
        if (_placeholdersShown == value) return;
        _placeholdersShown = value;
        NewBlockPlaceholderPool.ClearAll();
        if (value) NewBlockPlaceholderPool.CreateAround(this);
    }

    public void Destroy()
    {
        BindMatrix.RemoveAllBinds(this);
        Destroy(gameObject);
    }

    Text _text;
    public void DisplayText(string s)
    {
        if (s.Length > 3) s = "";
        if (_text)
            _text.text = s;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InputHandler.BlockClicked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputHandler.BlockClicked = false;
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
            PulseBlock.ColorPalette.SubscribeGameObject(_fieldCircle.gameObject, 1);
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
        RefreshFieldCircle();
    }

    public void OnUnbind(Bind b)
    {
        if (b.Second == this && b.First is Block) RefreshStepNumber();
        RefreshFieldCircle();
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

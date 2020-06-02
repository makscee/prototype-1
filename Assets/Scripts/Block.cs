﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Block : BindableMonoBehavior, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBindHandler
{
    int _x, _y;
    public GameObject inside;
    public GameObject text;

    public const float BlockSide = 1;

    public int X => _x;
    public int Y => _y;

    void SetCoords(int x, int y)
    {
        _x = x;
        _y = y;
        RevertToDefaultColor();
    }

    void RevertToDefaultColor()
    {
        var xt = _x + 1000000;
        var yt = _y + 1000000;
        ColorPalette.SubscribeGameObject(inside, 1 - (xt + yt) % 2);
    }

    void OnEnable()
    {
        ColorPalette.SubscribeGameObject(gameObject, 3);
        ColorPalette.SubscribeGameObject(text, 3);
        RevertToDefaultColor();
        _text = text.GetComponent<Text>();
    }

    protected override void Start()
    {
        base.Start();
        var rect = GetComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BlockSide);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BlockSide);
    }

    public static Block Create()
    {
        return Instantiate(Prefabs.Instance.Block, SharedObjects.Instance.Canvas.transform).GetComponent<Block>();
    }
    
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            BindMatrix.AddBind(this, MouseBind.Get(), Vector2.zero, Bind.MouseBindStrength);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            BindMatrix.AddBind(this, MouseBind.Get(), Vector2.zero, 0);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            BindMatrix.RemoveBind(this, MouseBind.Get());
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            var pos = MouseBind.Get().GetPosition();
            var size = new Vector2(BlockSide, BlockSide);
            var up = new Rect(GetPosition() + new Vector2(0, BlockSide) - size / 2, size);
            var down = new Rect(GetPosition() + new Vector2(0, -BlockSide) - size / 2, size);
            var left = new Rect(GetPosition() + new Vector2(-BlockSide, 0) - size / 2, size);
            var right = new Rect(GetPosition() + new Vector2(BlockSide, 0) - size / 2, size);

            Vector2 newBlockOffset;
            int x, y;
            if (up.Contains(pos))
            {
                newBlockOffset = Vector2.up * BlockSide;
                x = X;
                y = Y + 1;
            }
            else if (down.Contains(pos))
            {
                newBlockOffset = Vector2.down * BlockSide;
                x = X;
                y = Y - 1;
            }
            else if (left.Contains(pos))
            {
                newBlockOffset = Vector2.left * BlockSide;
                x = X - 1;
                y = Y;
            }
            else if (right.Contains(pos))
            {
                newBlockOffset = Vector2.right * BlockSide;
                x = X + 1;
                y = Y;
            }
            else
            {
                BindMatrix.RemoveBind(this, MouseBind.Get());
                return;
            }

            var existingBlock = FieldMatrix.Get(x, y); 
            if (existingBlock != null)
            {
                var bind = BindMatrix.GetBind(this, existingBlock);
                if (bind == null)
                    BindMatrix.AddBind(this, existingBlock, newBlockOffset, Bind.BlockBindStrength);
                else if (bind.First != this)
                {
                    bind.Break();
                    BindMatrix.AddBind(this, existingBlock, newBlockOffset, Bind.BlockBindStrength);
                }
            }
            else
            {
                var b = Create();
                b.transform.position = transform.position;
                b.SetCoords(x, y);
                FieldMatrix.Add(x, y, b);
                BindMatrix.AddBind(this, b, newBlockOffset, Bind.BlockBindStrength);
            }
            BindMatrix.RemoveBind(this, MouseBind.Get());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
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
        ColorPalette.SubscribeGameObject(inside, 3);
        GlobalPulse.SubscribeToNext(PassPulse);
    }

    public virtual void PassPulse()
    {
        bool passed = false;
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
            foreach (var last in _lastPulseFrom)
            {
                var v = transform.position - last.transform.position;
                var x = X - last.X;
                var y = Y - last.Y;
                transform.position += v;
                if (x > 0)
                    SoundsPlayer.Kick();
                else if (x < 0)
                    SoundsPlayer.Hat();
                else if (y > 0)
                    SoundsPlayer.Clap();
            }
        }
        _lastPulseFrom.Clear();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Destroy();
        }
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

    int _stepNumber = int.MaxValue;
    protected int StepNumber
    {
        get => _stepNumber;
        set
        {
            _stepNumber = value;
            DisplayText(_stepNumber.ToString());
        }
    }

    public void OnBind(Bind bind)
    {
        if (bind.Second == this && bind.First is Block block)
        {
            RefreshStepNumber();
        }
    }

    public void OnUnbind(Bind b)
    {
        if (b.Second != this || !(b.First is Block)) return;
        RefreshStepNumber();
    }

    public virtual void RefreshStepNumber()
    {
        var t = int.MaxValue;
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
}

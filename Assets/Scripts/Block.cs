using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Block : BindableMonoBehavior, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
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
        ColorPalette.SubscribeGameObject(text, 2 + (xt + yt) % 2);
    }

    void OnEnable()
    {
        ColorPalette.SubscribeGameObject(gameObject, 3);
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
        else
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
        else
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
                BindMatrix.AddBind(this, existingBlock, newBlockOffset, Bind.BlockBindStrength);
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
        StopCoroutine(nameof(PassCoroutine));
        StartCoroutine(nameof(PassCoroutine));
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

    protected IEnumerator PassCoroutine()
    {
        yield return new WaitForSeconds(PulseBlock.PulseDelay);
        PassPulse();
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
    public void DisplayText(string text)
    {
        _text.text = text;
    }
}

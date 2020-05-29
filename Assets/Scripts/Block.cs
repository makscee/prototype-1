using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : BindableMonoBehavior, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public int X, Y;
    public GameObject inside;

    protected const float BlockSide = 1;
    void OnEnable()
    {
        ColorPalette.SubscribeGameObject(gameObject, 3);
        ColorPalette.SubscribeGameObject(inside, 1);
    }

    protected override void Start()
    {
        base.Start();
        var blockSide = BlockSide;
        var rect = GetComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, blockSide);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, blockSide);
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
                b.X = x;
                b.Y = y;
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
        if (_lastPulseFrom.Count == 0)
        {
            ColorPalette.SubscribeGameObject(inside, 3);
            StartCoroutine(nameof(PassCoroutine));
        }
        _lastPulseFrom.Add(from);
    }

    public virtual void PassPulse()
    {
        bool passed = false;
        ColorPalette.SubscribeGameObject(inside, 1);
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
        {
            if ((bind.First == this ? bind.Second : bind.First) is Block block)
                if (!_lastPulseFrom.Contains(block))
                {
                    block.ReceivePulse(this);
                    passed = true;
                }
        }

        if (!passed)
        {
            var v = transform.position - _lastPulseFrom[0].transform.position;
            var parent = _lastPulseFrom[0];
            var x = X - parent.X;
            var y = Y - parent.Y;
            transform.position += v;
            if (x > 0)
                SoundsPlayer.Kick();
            else if (x < 0)
                SoundsPlayer.Hat();
            else if (y > 0)
                SoundsPlayer.Clap();
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
}

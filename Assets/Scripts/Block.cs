using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : BindableMonoBehavior, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public GameObject inside;

    public float BlockSide;
    void OnEnable()
    {
        ColorPalette.SubscribeGameObject(gameObject, 3);
        ColorPalette.SubscribeGameObject(inside, 1);
    }

    protected override void Start()
    {
        base.Start();
        var blockSide = TetrisField.Instance.BlockSide;
        var rect = GetComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, blockSide);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, blockSide);
    }

    public static Block Create()
    {
        return Instantiate(Prefabs.Instance.Block, SharedObjects.Instance.Canvas.transform).GetComponent<Block>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            BindMatrix.AddBind(this, MouseBind.Get(), Vector2.zero, MouseBindStrength);
        }
        else
        {
            BindMatrix.AddBind(this, MouseBind.Get(), Vector2.zero, 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            BindMatrix.RemoveBind(this, MouseBind.Get());
        }
        else
        {
            var pos = MouseBind.Get().GetPosition();
            var m = TetrisField.Instance.BlockSide;
            var size = new Vector2(m, m);
            var up = new Rect(GetPosition() + new Vector2(0, m) - size / 2, size);
            var down = new Rect(GetPosition() + new Vector2(0, -m) - size / 2, size);
            var left = new Rect(GetPosition() + new Vector2(-m, 0) - size / 2, size);
            var right = new Rect(GetPosition() + new Vector2(m, 0) - size / 2, size);

            Vector2 newBlockOffset;
            if (up.Contains(pos)) newBlockOffset = Vector2.up * m;
            else if (down.Contains(pos)) newBlockOffset = Vector2.down * m;
            else if (left.Contains(pos)) newBlockOffset = Vector2.left * m;
            else if (right.Contains(pos)) newBlockOffset = Vector2.right * m;
            else
            {
                BindMatrix.RemoveBind(this, MouseBind.Get());
                return;
            }

            var b = Create();
            b.transform.position = transform.position;
            BindMatrix.AddBind(this, b, newBlockOffset, BlockBindStrength);
            BindMatrix.RemoveBind(this, MouseBind.Get());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    protected override void FixedUpdate()
    {
        FieldCheck();
        base.FixedUpdate();
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

    bool _checkedAfterUnbound;

    void FieldCheck()
    {
        if (IsAnchored())
        {
            _checkedAfterUnbound = false;
            return;
        }
        if (_checkedAfterUnbound) return;

        _checkedAfterUnbound = true;
        foreach (var block in CollectBoundBlocks())
        {
            block.TryToStick();
        }
    }

    const int BlockBindStrength = 5;
    const int CellBindStrength = 5;
    const int MouseBindStrength = 7;

    void TryToStick()
    {
        if (!TetrisField.Instance.GetCoordsFromScreenPos(transform.position, out var x, out var y))
        {
            return;
        }
        
        BindMatrix.AddBind(this, TetrisField.Instance.GetCell(x, y), Vector2.zero, CellBindStrength, 50);
        
        var left = TetrisField.Instance.GetBlock(x - 1, y);
        var right = TetrisField.Instance.GetBlock(x + 1, y);
        var up = TetrisField.Instance.GetBlock(x, y + 1);
        var down = TetrisField.Instance.GetBlock(x, y - 1);

        var m = TetrisField.Instance.BlockSide;
        if (left != null)  BindMatrix.AddBind(this, left, Vector2.left * m, BlockBindStrength);
        if (right != null) BindMatrix.AddBind(this, right, Vector2.right * m, BlockBindStrength);
        if (up != null)  BindMatrix.AddBind(this, up, Vector2.up * m, BlockBindStrength);
        if (down != null) BindMatrix.AddBind(this, down, Vector2.down * m, BlockBindStrength);
    }
}

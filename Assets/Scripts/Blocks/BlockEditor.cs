using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class BlockEditor
{
    static HashSet<Block> _currentCluster;
    static Block _hovered;

    public static void OnBlockClick(Block block)
    {
        if (_currentCluster != null)
        {
            foreach (var b in _currentCluster)
                b.SetMasked(false);
            
            if (_currentCluster.Contains(block))
            {
                _currentCluster = null;
                return;
            }
        }
        var cluster = new HashSet<Block>();
        foreach (var obj in BindMatrix.CollectAllBoundObjects(block))
            if (obj is Block b)
            {
                cluster.Add(b);
                b.SetMasked(true);
            }
        _currentCluster = cluster;
    }

    public static void OnBlockDragStart(Block block, PointerEventData eventData)
    {
        _hovered = block;
    }

    public static void OnBlockDrag(PointerEventData eventData)
    {
        GetInputCoords(out var x, out var y);
        if (FieldMatrix.Get(x, y, out var blockOnInput))
        {
            if (blockOnInput == _hovered) return;
            var bind = BindMatrix.GetBind(_hovered, blockOnInput);
            var newBlockOffset = new Vector2(x - _hovered.X, y - _hovered.Y);
            if (bind == null)
                BindMatrix.AddBind(_hovered, blockOnInput, newBlockOffset, Bind.BlockBindStrength);
            else if (bind.First != _hovered)
            {
                if (BindMatrix.GetOutBindsCount(_hovered) == 0)
                {
                    _hovered.Destroy();
                    _hovered = blockOnInput;
                    return;
                }
                
                bind.Break();
                BindMatrix.AddBind(_hovered, blockOnInput, newBlockOffset, Bind.BlockBindStrength);
            } else if (bind.First == _hovered)
                bind.Break();

            _hovered = blockOnInput;
            _hovered.SetMasked(true);
            return;
        }

        var newBlock = Block.Create(_hovered, x, y);
        _hovered = newBlock;
        newBlock.SetMasked(true);
        _currentCluster.Add(newBlock);
    }

    static void GetInputCoords(out int x, out int y)
    {
        var worldPos = SharedObjects.Instance.Camera.ScreenToWorldPoint(Input.mousePosition);
        x = (int) Math.Round(worldPos.x);
        y = (int) Math.Round(worldPos.y);
    }
}
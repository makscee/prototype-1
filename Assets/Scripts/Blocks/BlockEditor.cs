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
                if (b != null)
                    b.SetMasked(false);
            
            if (_currentCluster.Contains(block))
            {
                _currentCluster = null;
                return;
            }
        }
        var cluster = new HashSet<Block>();
        foreach (var obj in BindMatrix.CollectBoundCluster(block))
            if (obj is Block b)
            {
                cluster.Add(b);
                b.SetMasked(true);
            }
        _currentCluster = cluster;
    }

    public static void OnBlockDragStart(Block block)
    {
        _hovered = block;
    }

    public static Block OnBlockDrag()
    {
        Utils.GetInputCoords(out var x, out var y);
        if (FieldMatrix.Get(x, y, out var blockOnInput))
        {
            if (blockOnInput == _hovered) return _hovered;
            var bind = BindMatrix.GetBind(_hovered, blockOnInput);
            var newBlockOffset = new Vector2(x - _hovered.X, y - _hovered.Y);
            if (bind == null)
                BindMatrix.AddBind(_hovered, blockOnInput, newBlockOffset, Bind.BlockBindStrength);
            else if (bind.First != _hovered)
            {
                if (BindMatrix.GetOutBindsCount(_hovered) == 0)
                {
                    _hovered.SetMasked(false);
                    _hovered.Destroy();
                    _hovered = blockOnInput;
                    return _hovered;
                }
                
                bind.Break();
                BindMatrix.AddBind(_hovered, blockOnInput, newBlockOffset, Bind.BlockBindStrength);
            } else if (bind.First == _hovered)
                bind.Break();

            _hovered = blockOnInput;
            _hovered.SetMasked(true);
            return _hovered;
        }

        _hovered = CreatePath(_hovered, x, y);
        return _hovered;
    }

    static Block CreatePath(Block block, int x, int y)
    {
        var xTotal = Mathf.Abs(x - block.X);
        var yTotal = Mathf.Abs(y - block.Y);
        var xDelta = xTotal > 0 ? (x - block.X) / xTotal : 0;
        var yDelta = yTotal > 0 ? (y - block.Y) / yTotal : 0;
        int xCur = block.X, yCur = block.Y;
        var parentBlock = block;
        var xPerc = xTotal != 0 ? Mathf.Abs((float) xCur - block.X) / xTotal : 1f;
        var yPerc = yTotal != 0 ? Mathf.Abs((float) yCur - block.Y) / yTotal : 1f;
        for (var i = 0; i < xTotal + yTotal; i++)
        {
            if (xPerc > yPerc)
            {
                yCur += yDelta;
                yPerc = Mathf.Abs((float) yCur - block.Y) / yTotal;
            }
            else
            {
                xCur += xDelta;
                xPerc = Mathf.Abs((float) xCur - block.X) / xTotal;
            }
            if (FieldMatrix.Get(xCur, yCur, out var b))
            {
                parentBlock = b;
            }
            else
            {
                var newBlock = Block.Create(parentBlock, xCur, yCur);
                parentBlock = newBlock;
                newBlock.SetMasked(true);
                _currentCluster.Add(newBlock);
            }
        }

        return parentBlock;
    }
}
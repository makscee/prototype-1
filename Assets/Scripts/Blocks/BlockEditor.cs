using System.Collections.Generic;
using UnityEngine;

public static class BlockEditor
{
    static HashSet<BlockOld> _currentCluster;
    static BlockOld _hovered;
    public static PulseBlock ClusterPulseBlock;

    public static void OnBlockClick(BlockOld block)
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
        var cluster = new HashSet<BlockOld>();
        foreach (var obj in BindMatrix.CollectBoundCluster(block))
            if (obj is BlockOld b)
            {
                cluster.Add(b);
                b.SetMasked(true);
                if (obj is PulseBlock pulseBlock)
                    ClusterPulseBlock = pulseBlock;
            }
        _currentCluster = cluster;
    }

    public static void OnBlockDragStart(BlockOld block)
    {
        Hovered = block;
    }

    public static BlockOld OnBlockDrag()
    {
        Utils.GetInputCoords(out var x, out var y);
        var blockOnInput = new BlockOld();
        // if (FieldMatrix.Get(x, y, out var blockOnInput))
        {
            if (blockOnInput == Hovered) return Hovered;
            var bind = BindMatrix.GetBind(Hovered, blockOnInput);
            var newBlockOffset = new Vector2(x - Hovered.X, y - Hovered.Y);
            if (bind == null)
                BindMatrix.AddBind(Hovered, blockOnInput, newBlockOffset, Bind.BlockBindStrength);
            else if (bind.First != Hovered)
            {
                if (BindMatrix.GetOutBindsCount(Hovered) == 0)
                {
                    Hovered.SetMasked(false);
                    Hovered.Destroy();
                    Hovered = blockOnInput;
                    return Hovered;
                }
                
                bind.Break();
                BindMatrix.AddBind(Hovered, blockOnInput, newBlockOffset, Bind.BlockBindStrength);
            } else if (bind.First == Hovered)
                bind.Break();

            Hovered = blockOnInput;
            Hovered.SetMasked(true);
            return Hovered;
        }

        Hovered = CreatePath(Hovered, x, y);
        return Hovered;
    }

    public static bool HasActiveCluster => _currentCluster != null;

    static BlockOld Hovered
    {
        get => _hovered;
        set
        {
            if (_hovered != null)
                PixelFieldMatrix.Show(_hovered.X, _hovered.Y, Color.red);
            _hovered = value;
            if (_hovered != null)
                PixelFieldMatrix.Show(_hovered.X, _hovered.Y, Color.blue);
        }
    }

    public static void DeselectCurrent()
    {
        if (_currentCluster == null) return;
        foreach (var b in _currentCluster)
            if (b != null)
                b.SetMasked(false);
        _currentCluster = null;
    }

    static BlockOld CreatePath(BlockOld block, int x, int y)
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
            // if (FieldMatrix.Get(xCur, yCur, out var b))
            // {
            //     parentBlock = b;
            // }
            // else
            {
                var newBlock = BlockOld.Create(parentBlock, xCur, yCur);
                parentBlock = newBlock;
                newBlock.SetMasked(true);
                _currentCluster.Add(newBlock);
            }
        }

        return parentBlock;
    }
}
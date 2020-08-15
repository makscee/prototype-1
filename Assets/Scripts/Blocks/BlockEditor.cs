using UnityEngine;
using UnityEngine.EventSystems;

public static class BlockEditor
{
    static int _lastX, _lastY;
    static bool _blockDragged;
    public static void OnBlockDragStart(Block block)
    {
        _lastX = block.logic.X;
        _lastY = block.logic.Y;
        _blockDragged = true;
    }

    public static void OnBlockDrag()
    {
        if (!_blockDragged) return;
        Utils.GetInputCoords(out var x, out var y);
        CreatePath(_lastX, _lastY, x, y);
        _lastX = x;
        _lastY = y;
    }

    public static void OnBlockDragEnd()
    {
        _blockDragged = false;
    }

    static void DragFromTo(int fromX, int fromY, int toX, int toY)
    {
        if (!FieldMatrix.Get(fromX, fromY, out var fromBlock)) return;
        if (FieldMatrix.Get(toX, toY, out var toBlock))
        {
            var bind = BindMatrix.GetBind(fromBlock, toBlock);
            var newBlockOffset = new Vector2(toX - fromX, toY - fromY);
            if (bind == null)
            {
                BindMatrix.AddBind(fromBlock, toBlock, newBlockOffset, Bind.BlockBindStrength);
                return;
            }

            if (bind.First != fromBlock)
            {
                if (!(fromBlock is RootBlock) && BindMatrix.GetOutBindsCount(fromBlock) == 0)
                {
                    fromBlock.Destroy();
                    return;
                }
                
                bind.Break();
                BindMatrix.AddBind(fromBlock, toBlock, newBlockOffset, Bind.BlockBindStrength);
                return;
            }
            return;
        }
        NodeBlock.Create(toX, toY, fromBlock);
    }

    static void CreatePath(int fromX, int fromY, int x, int y)
    {
        var xTotal = Mathf.Abs(x - fromX);
        var yTotal = Mathf.Abs(y - fromY);
        var xDelta = xTotal > 0 ? (x - fromX) / xTotal : 0;
        var yDelta = yTotal > 0 ? (y - fromY) / yTotal : 0;
        int xCur = fromX, yCur = fromY;
        var xPerc = xTotal != 0 ? Mathf.Abs((float) xCur - fromX) / xTotal : 1f;
        var yPerc = yTotal != 0 ? Mathf.Abs((float) yCur - fromY) / yTotal : 1f;
        for (var i = 0; i < xTotal + yTotal; i++)
        {
            int xFrom = xCur, yFrom = yCur;
            if (xPerc > yPerc)
            {
                yCur += yDelta;
                yPerc = Mathf.Abs((float) yCur - fromY) / yTotal;
            }
            else
            {
                xCur += xDelta;
                xPerc = Mathf.Abs((float) xCur - fromX) / xTotal;
            }
            DragFromTo(xFrom, yFrom, xCur, yCur);
        }
    }
}
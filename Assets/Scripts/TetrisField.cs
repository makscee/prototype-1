using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisField : MonoBehaviour
{
    Cell[,] _cells;
    Block[,] _blocks;
    
    public Cell CellPrefab;
    public RectTransform RectTransform;

    
    int W = 8, H;

    [NonSerialized]
    public float BlockSide;

    public static TetrisField Instance;
    void OnEnable()
    {
        Instance = this;
        var ownRect = RectTransform.rect;
        BlockSide = ownRect.width / W;
        H = (int)(ownRect.height / BlockSide);
        _cells = new Cell[W,H];
        _blocks = new Block[W,H];
        for (var x = 0; x < W; x++)
        {
            for (var y = 0; y < H; y++)
            {
                var cell = Instantiate(CellPrefab, transform).GetComponent<Cell>();
                _cells[x, y] = cell;
                cell.field = this;
                cell.x = x;
                cell.y = y;
                var rect = cell.GetComponent<RectTransform>();  
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BlockSide);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BlockSide);
                var scale = BlockSide;
                float xPos = x * scale + 0.5f * BlockSide, yPos = y * scale + 0.5f * BlockSide;
                rect.anchoredPosition = new Vector2(xPos, yPos);
            }
        }
    }

    public bool GetCoords(Block block, out int x, out int y)
    {
        x = -1;
        y = -1;
        for (var i = 0; i < W; i++)
        {
            for (var j = 0; j < H; j++)
            {
                if (_blocks[i, j] == block)
                {
                    x = i;
                    y = j;
                    return true;
                }
            }
        }

        return false;
    }

    public bool GetCoordsFromScreenPos(Vector2 pos, out int x, out int y)
    {
        x = -1;
        y = -1;
        if (!RectTransform.rect.Contains(pos - new Vector2(Screen.width / 2f, Screen.height / 2f))) return false;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, pos, null, out var v);
        v += RectTransform.rect.size / 2;
        v /= BlockSide;

        x = (int) v.x;
        y = (int) v.y;
        return true;
    }

    public Cell GetCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= W || y >= H) return null;
        return _cells[x, y];
    }

    public Block GetBlock(int x, int y)
    {
        if (x < 0 || y < 0 || x >= W || y >= H) return null;
        return _blocks[x, y];
    }

    public void AddBlock(int x, int y, Block block)
    {
        if (_blocks[x, y] != null && _blocks[x, y] != block) Debug.LogError($"Block exists on {x}, {y}");
        _blocks[x, y] = block;
        // block.Bind(_cells[x, y], Vector2.zero, 1, 50);
    }

    public void RemoveBlock(int x, int y)
    {
        if (_blocks[x, y] == null) Debug.LogError($"No block at {x}, {y}");
        _blocks[x, y] = null;
    }

    void OnDisable()
    {
        foreach (var cell in _cells)
        {
            if (!cell) continue;;
            Destroy(cell);
        }
    }

    
}

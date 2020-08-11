using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeBlock : Block
{
    void OnEnable()
    {
        view.onRefresh += () => view.secondaryPainter.NumInPalette = logic.HasPulse ? 3 : CheckerboardColor;
        logic.onPulseReceive += OnPulseDeadEnd;
    }

    void OnPulseDeadEnd(Block from)
    {
        if (from == null || BindMatrix.GetOutBindsCount(this) != 0) return;
        var dir = Utils.DirFromCoords(logic.Position - from.logic.Position);
        SharedObjects.Instance.rootBlocks[rootDirection].soundsPlayer.Play(dir);
    }

    public static NodeBlock Create(int x, int y, int rootDirection = 0)
    {
        var b = Instantiate(Prefabs.Instance.NodeBlock, SharedObjects.Instance.rootCanvases[rootDirection].transform).GetComponent<NodeBlock>();
        b.rootDirection = rootDirection;
        b.logic.SetCoords(x, y);
        b.IsAnchored = true;
        BindMatrix.AddBind(StaticAnchor.Create(b.logic.Position), b, Vector2.zero, Bind.BlockStaticBindStrength);
        b.view.SetDirty();
        b.logic.onTap += e => b.logic.ReceivePulse();
        b.logic.onTap += e =>
        {
            if (e.button == PointerEventData.InputButton.Middle) b.Destroy();
        };
        return b;
    }

    public static NodeBlock Create(int x, int y, Block boundWith)
    {
        var b = Create(x, y, boundWith.rootDirection);
        BindMatrix.AddBind(boundWith, b, b.logic.Position - boundWith.logic.Position, Bind.BlockBindStrength);
        return b;
    }

    int CheckerboardColor
    {
        get
        {
            var xt = logic.X + 1000000;
            var yt = logic.Y + 1000000;
            return 1 - (xt + yt) % 2;
        }
    }
}
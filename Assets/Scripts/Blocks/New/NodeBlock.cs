using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeBlock : Block
{
    protected override void OnEnable()
    {
        base.OnEnable();
        logic.onTap += e => logic.ReceivePulse();
        logic.onTap += e =>
        {
            if (e.button == PointerEventData.InputButton.Middle) Destroy();
        };
        logic.onPulseReceive += OnPulseDeadEnd;
        view.onRefresh += () => view.secondaryPainter.NumInPalette = logic.HasPulse ? 3 : CheckerboardColor;
        view.SetDirty();
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
        b.transform.position = new Vector3(x, y);
        BindMatrix.AddBind(StaticAnchor.Create(b.logic.Position), b, Vector2.zero, Bind.BlockStaticBindStrength);
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
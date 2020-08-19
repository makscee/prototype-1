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
        view.SetInitialModel(BlockVisualBase.Model.NodeDeadend);
        view.onRefresh += () =>
        {
            view.VisualBase.Select(BindMatrix.GetOutBindsCount(this) == 0
                ? BlockVisualBase.Model.NodeDeadend
                : BlockVisualBase.Model.NodePipe);
            view.SecondaryPainter.NumInPalette = logic.HasPulse ? 3 : CheckerboardColor;
        };
        view.SetDirty();
    }

    void OnPulseDeadEnd(Block from)
    {
        if (from == null || BindMatrix.GetOutBindsCount(this) != 0) return;
        var dir = Utils.DirFromCoords(logic.Position - from.logic.Position);
        SharedObjects.Instance.rootBlocks[rootDirection].soundsPlayer.Play(dir);
        PixelDriver.Add(PixelRoad.Circle(SharedObjects.Instance.rootBlocks[0].view.PrimaryPainter.palette.GetColor(3),
            3f, 3f, 0.05f, 0.5f, logic.X, logic.Y).SetWeight(0.05f));
    }

    public static NodeBlock Create(int x, int y, int rootDirection = 0)
    {
        var b = Instantiate(Prefabs.Instance.nodeBlock, SharedObjects.Instance.rootCanvases[rootDirection].transform).GetComponent<NodeBlock>();
        b.rootDirection = rootDirection;
        b.logic.SetCoords(x, y);
        b.transform.position = new Vector3(x, y);
        BindMatrix.AddBind(StaticAnchor.Create(b.logic.Position, false), b, Vector2.zero, Bind.BlockStaticBindStrength);
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
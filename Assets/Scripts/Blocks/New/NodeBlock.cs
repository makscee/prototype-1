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
        logic.onBind += bind =>
        {
            if (bind.Second == this && bind.First is Block)
                RefreshStepNumber();
        };
        logic.onUnbind += bind =>
        {

            if (bind.Second == this && bind.First is Block)
                RefreshStepNumber();
        };
    }

    protected override void StartInit()
    {
        base.StartInit();
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
        var root = Roots.Blocks[rootNum]; 
        root.soundsPlayer.Play(dir); 
        PixelDriver.Add(PixelRoad.Circle(root.view.PrimaryPainter.palette.GetColor(dir),
            2f, 3f, 0.05f, 0.5f, logic.X, logic.Y).SetWeight(0.3f));
    }
    
    public void RefreshStepNumber()
    {
        var t = int.MaxValue / 2;
        if (IsAnchored)
        {
            foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
            {
                if (bind.Second == this && bind.First is Block block)
                {
                    t = Math.Min(t, block.logic.stepNumber + 1);
                }
            }
        }

        if (t == logic.stepNumber)
            return;
        logic.stepNumber = t;
        view.SetText(logic.stepNumber.ToString());
        StepNumberChangeNotify();
    }

    public void StepNumberChangeNotify()
    {
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
        {
            if (bind.First == this && bind.Second is NodeBlock nodeBlock)
            {
                nodeBlock.RefreshStepNumber();
            }
        }
    }

    public static NodeBlock Create(int x, int y, int rootId = 0, float startOffsetClamp = 2f)
    {
        var position = new Vector2(x, y);
        var startPosition = position + Vector2.ClampMagnitude(Roots.Blocks[rootId].logic.Position - position, startOffsetClamp);
        var b = Instantiate(Prefabs.Instance.nodeBlock, startPosition, Quaternion.identity, Roots.RootCanvases(rootId).transform).GetComponent<NodeBlock>();
        b.rootNum = rootId;
        b.logic.SetCoords(x, y);
        b.StartInit();
        BindMatrix.AddBind(StaticAnchor.Create(b.logic.Position, false), b, Vector2.zero, Bind.BlockStaticBindStrength);
        return b;
    }

    public static NodeBlock Create(int x, int y, Block boundWith)
    {
        var b = Create(x, y, boundWith.rootNum);
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
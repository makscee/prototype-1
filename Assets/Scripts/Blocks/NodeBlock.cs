using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

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
        logic.onTap += e => OnDeadendTap();
        
        logic.onPulseReceive += OnPulseDeadEnd;
        logic.onBind += bind =>
        {
            UpdateDirs();
            if (bind.Second == this && bind.First is Block block && block.rootId == rootId)
            {
                pulseVersion = block.pulseVersion;
                RefreshStepNumber();
            }
        };
        logic.onUnbind += bind =>
        {
            UpdateDirs();
            if (bind.Second == this && bind.First is Block)
                RefreshStepNumber();
            var alone = true;
            foreach (var adjacentBind in BindMatrix.GetAllAdjacentBinds(this))
            {
                if (adjacentBind.First is Block && adjacentBind.First != this ||
                    adjacentBind.Second is Block && adjacentBind.Second != this)
                {
                    alone = false;
                    break;
                }
            }
            if (alone) Destroy();
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

    public readonly bool[] dirs = new bool[4];
    public bool DeadEnd => BindMatrix.GetOutBindsCount(this) == 0;
    void UpdateDirs()
    {
        if (!DeadEnd)
        {
            for (var i = 0; i < 4; i++)
                dirs[i] = false;
            return;
        }
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
            if (bind.First is Block block && bind.Second == this)
                dirs[Utils.DirFromCoords(logic.Position - block.logic.Position)] = true;
    }

    void Update()
    {
        if (_stepNumberDirty) RefreshStepNumber();
    }

    void OnDeadendTap()
    {
        if (!DeadEnd) return;
        var d = new List<int>();
        for (var i = 0; i < 4; i++)
        {
            if (dirs[i]) d.Add(i);
        }
        Roots.Root[rootId].directionPanelsGroup.OpenOneCloseRest(Roots.Root[rootId].directionPanels[d[Random.Range(0, d.Count)]]);
        Roots.RootPanelsGroup.OpenOneCloseRest(Roots.Root[rootId].rootPanelsFolder);
    }

    void OnPulseDeadEnd(Block from)
    {
        if (from == null || BindMatrix.GetOutBindsCount(this) != 0) return;
        var dir = Utils.DirFromCoords(logic.Position - from.logic.Position);
        var root = Roots.Root[rootId].block; 
        root.soundsPlayer.Play(dir); 
        PixelDriver.Add(PixelRoad.Circle(root.view.PrimaryPainter.palette.GetColor(dir),
            2f, 3f, 0.05f, 0.5f, logic.X, logic.Y).SetWeight(0.3f));
    }
    public void RefreshStepNumber()
    {
        var t = int.MaxValue / 2;
        if (PulseConnected)
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

    bool _stepNumberDirty;
    public void StepNumberChangeNotify()
    {
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
        {
            if (bind.First == this && bind.Second is NodeBlock nodeBlock)
            {
                nodeBlock._stepNumberDirty = true;
            }
        }
    }

    public static NodeBlock Create(int x, int y, int rootId = 0, float startOffsetClamp = 2f)
    {
        var position = new Vector2(x, y);
        var startPosition = position + Vector2.ClampMagnitude(Roots.Root[rootId].block.logic.Position - position, startOffsetClamp);
        var b = Instantiate(Prefabs.Instance.nodeBlock, startPosition, Quaternion.identity, Roots.Root[rootId].rootCanvas.transform).GetComponent<NodeBlock>();
        b.rootId = rootId;
        b.logic.SetCoords(x, y);
        b.StartInit();
        var startModel = b.view.VisualBase.Current;
        Animator.Interpolate(0.4f, 1f, 0.4f)
            .PassValue(v => startModel.transform.localScale = new Vector3(v, v, 1))
            .Type(InterpolationType.InvSquare).NullCheck(startModel.gameObject);
        BindMatrix.AddBind(StaticAnchor.Create(b.logic.Position, false), b, Vector2.zero, Bind.BlockStaticBindStrength);
        Roots.Root[rootId].DeadEndsSetDirty();
        return b;
    }

    public static NodeBlock Create(int x, int y, Block boundWith)
    {
        var b = Create(x, y, boundWith.rootId);
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
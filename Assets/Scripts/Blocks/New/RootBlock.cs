using System;
using UnityEngine;

public class RootBlock : Block
{
    public SoundsPlayer soundsPlayer;

    protected override void OnEnable()
    {
        logic.onTap += e =>
        {
            if (BindMatrix.GetOutBindsCount(this) == 0)
            {
                var v = logic.Position + Utils.CoordsFromDir(rootNum % 4);
                NodeBlock.Create(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), this);
            } else logic.ReceivePulse();
        };
        view.onRefresh += () => view.SecondaryPainter.NumInPalette = logic.HasPulse ? 3 : 2;
        logic.stepNumber = 0;
    }

    protected override void StartInit()
    {
        base.StartInit();
        view.SetInitialModel(BlockVisualBase.Model.Root);
        BindMatrix.AddBind(StaticAnchor.Create(logic.Position), this, Vector2.zero, Bind.BlockBindStrength);
    }

    public static RootBlock Create(int x, int y, int rootNum = -1)
    {
        rootNum = rootNum == -1 ? Roots.Count : rootNum;
        var b = Instantiate(Prefabs.Instance.rootBlock, Roots.RootCanvases(rootNum).transform).GetComponent<RootBlock>();
        b.rootNum = rootNum;
        Roots.Blocks[rootNum] = b;
        b.logic.SetCoords(x, y);
        b.transform.position = b.logic.Position;
        b.IsAnchored = true;
        b.StartInit();
        BindMatrix.AddBind(StaticAnchor.Create(b.logic.Position), b, Vector2.zero, Bind.BlockStaticBindStrength);
        b.view.SetDirty();
        return b;
    }
}
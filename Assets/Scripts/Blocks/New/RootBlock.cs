using System;
using UnityEngine;

public class RootBlock : Block
{
    public SoundsPlayer soundsPlayer;
    public int direction;

    protected override void OnEnable()
    {
        rootDirection = direction;
        SharedObjects.Instance.rootBlocks[direction] = this;
        
        BindMatrix.AddBind(StaticAnchor.Create(logic.Position), this, Vector2.zero, Bind.BlockBindStrength);
        
        logic.onTap += e =>
        {
            if (BindMatrix.GetOutBindsCount(this) == 0)
            {
                var v = logic.Position + Utils.CoordsFromDir(direction);
                NodeBlock.Create(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), this);
            } else logic.ReceivePulse();
        };
        view.onRefresh += () => view.secondaryPainter.NumInPalette = logic.HasPulse ? 3 : 2;
        view.SetInitialModel(BlockVisualBase.Model.Root);
    }

    public static RootBlock Create(int x, int y, int dir)
    {
        var b = Instantiate(Prefabs.Instance.rootBlock, SharedObjects.Instance.rootCanvases[dir].transform).GetComponent<RootBlock>();
        b.direction = dir;
        b.logic.SetCoords(x, y);
        b.IsAnchored = true;
        BindMatrix.AddBind(StaticAnchor.Create(b.logic.Position), b, Vector2.zero, Bind.BlockStaticBindStrength);
        b.view.SetDirty();
        return b;
    }
}
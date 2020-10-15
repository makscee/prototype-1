using System;
using System.Collections.Generic;
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
                var v = logic.Position + Utils.CoordsFromDir(rootId % 4);
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

    void Update()
    {
        if (pulseVersionDirty) PulseConnectionUpdate();
    }

    public bool pulseVersionDirty;
    public void PulseConnectionUpdate()
    {
        pulseVersionDirty = false;
        pulseVersion++;
        
        var queue = new Queue<Block>();
        queue.Enqueue(this);
        while (queue.Count > 0)
        {
            var next = queue.Dequeue();
            next.pulseVersion = pulseVersion;
            foreach (var bind in BindMatrix.GetAllAdjacentBinds(next))
            {
                if (bind.First == next && bind.Second is Block block && block.pulseVersion != pulseVersion && block.rootId == rootId)
                    queue.Enqueue(block);
            }
        }
    }

    public static RootBlock Create(int x, int y, int rootId = -1)
    {
        rootId = rootId == -1 ? Roots.Count : rootId;
        var b = Instantiate(Prefabs.Instance.rootBlock, Roots.Root[rootId].rootCanvas.transform).GetComponent<RootBlock>();
        b.rootId = rootId;
        b.logic.SetCoords(x, y);
        b.transform.position = b.logic.Position;
        b.StartInit();
        BindMatrix.AddBind(StaticAnchor.Create(b.logic.Position), b, Vector2.zero, Bind.BlockStaticBindStrength);
        b.view.SetDirty();
        return b;
    }
}
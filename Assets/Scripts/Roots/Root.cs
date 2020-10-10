using System.Collections.Generic;
using UnityEngine;

public class Root
{
    public int id;
    public RootBlock block;
    public GameObject rootCanvas, visualsCanvas;
    public SlidingPanelsFolder rootPanelsFolder;
    public SlidingPanelsGroup directionPanelsGroup;
    public readonly SlidingPanelsFolder[] directionPanels = new SlidingPanelsFolder[4];
    public readonly WavePartsContainer[] wavePartsContainers = new WavePartsContainer[4];
    public Palette palette;
    public SlicedAudioClip slicedClip;
    List<NodeBlock> _deadEnds;
    public List<NodeBlock> DeadEnds
    {
        get
        {
            if (_deadEnds == null)
            {
                _deadEnds = new List<NodeBlock>();
                foreach (var b in FieldMatrix.GetAllAsList())
                    if (b is NodeBlock node && node.rootId == id && node.DeadEnd)
                        _deadEnds.Add(node);
            }
            return _deadEnds;
        }
    }

    public void DeadEndsSetDirty()
    {
        _deadEnds = null;
    }

    public void Destroy()
    {
        if (block != null) block.Destroy();
        Object.Destroy(rootCanvas);
        Object.Destroy(visualsCanvas);
        if (rootPanelsFolder != null) Object.Destroy(rootPanelsFolder.gameObject);
        if (directionPanelsGroup != null) Object.Destroy(directionPanelsGroup.gameObject);
        Roots.Root.Remove(id);
    }
}
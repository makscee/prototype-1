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

    public void Destroy()
    {
        if (block != null) block.Destroy();
        Object.Destroy(rootCanvas);
        Object.Destroy(visualsCanvas);
        Object.Destroy(rootPanelsFolder.gameObject);
        if (directionPanelsGroup != null) Object.Destroy(directionPanelsGroup.gameObject);
        Roots.Root.Remove(id);
    }
}
using UnityEngine;

public class Root
{
    public RootBlock block;
    public GameObject rootCanvas, visualsCanvas;
    public SlidingPanelsFolder rootPanelsFolder;
    public SlidingPanelsGroup directionPanelsGroup;
    public readonly SlidingPanelsFolder[] directionPanels = new SlidingPanelsFolder[4];
    public readonly WavePartsContainer[] wavePartsContainers = new WavePartsContainer[4];
    public Palette palette;
    public SlicedAudioClip slicedClip;
}
using UnityEngine;

public class Root
{
    public RootBlock block;
    public GameObject rootCanvas, visualsCanvas;
    public SlidingPanelsFolder rootPanelsFolder;
    public SlidingPanelsGroup directionPanelsGroup;
    public SlidingPanelsFolder[] directionPanels = new SlidingPanelsFolder[4];
    public Palette palette;
    public SlicedAudioClip slicedClip;
}
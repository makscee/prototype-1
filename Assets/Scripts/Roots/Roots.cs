using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Roots
{
    public static int Count => Root.Count;

    public static readonly Dictionary<int, Root> Root = new Dictionary<int, Root>();

    public static void CreateRoot(int x, int y, int id = -1, int colorsId = -1)
    {
        if (id == -1) id = Root.Count;
        if (colorsId == -1) colorsId = Colors.GetRandomFreeId();
        var root = new Root();
        root.id = id;
        Root.Add(id, root);

        CreateRootCanvas(id, colorsId);
        CreateVisualsCanvas(id);
        
        var block = RootBlock.Create(x, y, id);
        root.block = block;
        
        CreateLeftRootPanel(id);
        CreateRightDirectionsPanels(id);
        CreateSlicedAudioClip(id);
    }

    static void CreateRootCanvas(int id, int colorsId)
    {
        var go = Object.Instantiate(Prefabs.Instance.rootCanvas);
        go.name = $"RootCanvas{id}";
        
        var palette = go.GetComponent<Palette>(); 
        palette.ColorsId = colorsId;

        Root[id].palette = palette;
        Root[id].rootCanvas = go;
    }

    static void CreateVisualsCanvas(int id)
    {
        var go = Object.Instantiate(Prefabs.Instance.blockVisualsCanvas);
        go.name = $"BlockVisualsCanvas{id}";
        
        var palette = go.GetComponent<Palette>();
        palette.copyOf = Root[id].palette;
        
        Root[id].visualsCanvas = go;
    }

    static SlidingPanelsGroup _rootPanelsGroupCache;
    public static SlidingPanelsGroup RootPanelsGroup
    {
        get
        {
            if (_rootPanelsGroupCache == null)
                _rootPanelsGroupCache =
                    GameObject.Find("/PanelsCanvasLeft/PanelsGroup").GetComponent<SlidingPanelsGroup>();
            return _rootPanelsGroupCache;
        }
    }

    static void CreateLeftRootPanel(int id)
    {
        var go = Object.Instantiate(Prefabs.Instance.leftRootPanel, RootPanelsGroup.transform);
        go.name = $"RootPanels{id}";

        RootPanelsGroup.GetComponent<SlidingPanelsGroup>().CollectChildren();
        go.GetComponent<Palette>().copyOf = Root[id].palette;
        go.GetComponent<RootIdHolder>().id = id;
        var folder = go.GetComponent<SlidingPanelsFolder>(); 
        folder.onOpen += () => Root[id].directionPanelsGroup.transform.SetAsLastSibling();
        
        Root[id].rootPanelsFolder = folder;
    }

    static Transform _rightDirectionsPanels;
    static void CreateRightDirectionsPanels(int id)
    {
        if (_rightDirectionsPanels == null) _rightDirectionsPanels = GameObject.Find("/PanelsCanvasRight").transform;

        var go = Object.Instantiate(Prefabs.Instance.rightPanelGroup, _rightDirectionsPanels);
        go.name = $"DirectionsPanelsGroup{id}";

        go.GetComponent<Palette>().copyOf = Root[id].palette;
        go.GetComponent<RootIdHolder>().id = id;

        Root[id].directionPanelsGroup = go.GetComponent<SlidingPanelsGroup>();
        foreach (var idHolder in go.GetComponentsInChildren<DirectionIdHolder>())
        {
            Root[id].directionPanels[idHolder.id] = idHolder.GetComponent<SlidingPanelsFolder>();
            Root[id].wavePartsContainers[idHolder.id] = idHolder.GetComponentInChildren<WavePartsContainer>();
        }
    }

    static void CreateSlicedAudioClip(int rootId, int clipId = 1)
    {
        Root[rootId].slicedClip = SlicedAudioClip.CreateFromAsset(Resources.Load<SlicedAudioClipAsset>($"SlicedClips/slicedClip{clipId}"));
    }
}
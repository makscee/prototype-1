using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Roots
{
    public static int Count => Blocks.Count;
    
    public static readonly Dictionary<int, RootBlock> Blocks = new Dictionary<int, RootBlock>();
    public static readonly Dictionary<int, GameObject> RootCanvases = new Dictionary<int, GameObject>();
    public static readonly Dictionary<int, GameObject> VisualCanvases = new Dictionary<int, GameObject>();
    public static readonly Dictionary<int, SlidingPanelsFolder> RootPanelsFolders = new Dictionary<int, SlidingPanelsFolder>();
    public static readonly Dictionary<int, SlidingPanelsGroup> DirectionsPanelsGroup = new Dictionary<int, SlidingPanelsGroup>();
    public static readonly Dictionary<int, Dictionary<int, SlidingPanelsFolder>> DirectionsFolders = new Dictionary<int, Dictionary<int, SlidingPanelsFolder>>();
    public static readonly Dictionary<int, Palette> Palettes = new Dictionary<int, Palette>();

    public static void CreateRoot(int x, int y, int id = -1, int colorsId = -1)
    {
        if (id == -1) id = Blocks.Count;
        if (colorsId == -1) colorsId = Colors.GetRandomFreeId();

        CreateRootCanvas(id, colorsId);
        CreateVisualsCanvas(id);
        
        var block = RootBlock.Create(x, y, id);
        Blocks.Add(id, block);
        
        CreateLeftRootPanel(id);
        CreateRightDirectionsPanels(id);
    }

    static void CreateRootCanvas(int id, int colorsId)
    {
        var go = Object.Instantiate(Prefabs.Instance.rootCanvas);
        go.name = $"RootCanvas{id}";
        
        var palette = go.GetComponent<Palette>(); 
        palette.ColorsId = colorsId;
        
        Palettes.Add(id, palette);
        RootCanvases.Add(id, go);
    }

    static void CreateVisualsCanvas(int id)
    {
        var go = Object.Instantiate(Prefabs.Instance.blockVisualsCanvas);
        go.name = $"BlockVisualsCanvas{id}";
        
        var palette = go.GetComponent<Palette>();
        palette.copyOf = Palettes[id];
        
        VisualCanvases.Add(id, go);
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
        go.GetComponent<Palette>().copyOf = Palettes[id];
        go.GetComponent<RootIdHolder>().id = id;
        var folder = go.GetComponent<SlidingPanelsFolder>(); 
        folder.onOpen += () => DirectionsPanelsGroup[id].transform.SetAsLastSibling();
        
        RootPanelsFolders.Add(id, folder);
    }

    static Transform _rightDirectionsPanels;
    static void CreateRightDirectionsPanels(int id)
    {
        if (_rightDirectionsPanels == null) _rightDirectionsPanels = GameObject.Find("/PanelsCanvasRight").transform;

        var go = Object.Instantiate(Prefabs.Instance.rightPanelGroup, _rightDirectionsPanels);
        go.name = $"DirectionsPanelsGroup{id}";

        go.GetComponent<Palette>().copyOf = Palettes[id];
        go.GetComponent<RootIdHolder>().id = id;
        
        DirectionsPanelsGroup.Add(id, go.GetComponent<SlidingPanelsGroup>());
        DirectionsFolders.Add(id, new Dictionary<int, SlidingPanelsFolder>());
        foreach (var idHolder in go.GetComponentsInChildren<DirectionIdHolder>())
        {
            DirectionsFolders[id].Add(idHolder.id, idHolder.GetComponent<SlidingPanelsFolder>());
        }
    }
}
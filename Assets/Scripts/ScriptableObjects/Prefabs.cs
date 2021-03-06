using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs", menuName = "ScriptableObjects/Prefabs")]
public class Prefabs : ScriptableObject
{
    public GameObject nodeBlock;
    public GameObject rootBlock;
    public GameObject bindVisual;
    public GameObject pixel;
    public GameObject rootCanvas;
    public GameObject blockVisualsCanvas;
    public GameObject leftRootPanel;
    public GameObject rightPanelGroup;
    public GameObject duoLine;
    
    #region BlockVisualModels

    public GameObject blockVisualBase;
    public GameObject nodeDeadendVisualModel;
    public GameObject nodePipeVisualModel;
    public GameObject rootBlockVisualModel;

    #endregion

    #region Effects

    public GameObject FadeoutCircle;

    #endregion
    
    public static Prefabs Instance => GetInstance();

    static Prefabs _instanceCache;
    static Prefabs GetInstance()
    {
        if (_instanceCache == null)
            _instanceCache = Resources.Load<Prefabs>("Prefabs");
        return _instanceCache;
    }
}
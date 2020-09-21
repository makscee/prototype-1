using UnityEngine;

public class Prefabs : MonoBehaviour
{
    public GameObject nodeBlock;
    public GameObject rootBlock;
    public GameObject bindVisual;
    public GameObject pixel;
    public GameObject rollingButton;

    #region BlockVisualModels

    public GameObject blockVisualBase;
    public GameObject nodeDeadendVisualModel;
    public GameObject nodePipeVisualModel;
    public GameObject rootBlockVisualModel;

    #endregion
    
    
    public static Prefabs Instance;

    void OnEnable()
    {
        Instance = this;
    }
}

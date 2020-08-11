using UnityEngine;

public class SharedObjects : MonoBehaviour
{
    public GameObject BackCanvas;
    public GameObject MidCanvas;
    public GameObject FrontCanvas;
    public GameObject UICanvas;
    public Camera Camera;

    public GameObject[] rootCanvases;
    public RootBlock[] rootBlocks;
    public GameObject bindVisualsCanvas;
    public WaveEditor WaveEditor;

    public static SharedObjects Instance;

    void OnEnable()
    {
        Instance = this;
    }
}
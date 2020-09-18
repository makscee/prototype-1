using UnityEngine;

public class SharedObjects : MonoBehaviour
{
    public GameObject MidCanvas;
    public GameObject FrontCanvas;
    public Camera Camera;

    public GameObject[] blockVisualCanvases;
    public GameObject bindVisualsCanvas;
    public ConfigCanvas configCanvas;

    public BackgroundInputHandler backgroundInputHandler;

    public static SharedObjects Instance;

    void OnEnable()
    {
        Instance = this;
    }
}
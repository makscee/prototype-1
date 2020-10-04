using UnityEngine;

public class SharedObjects : MonoBehaviour
{
    public Camera Camera;

    public GameObject bindVisualsCanvas;

    public BackgroundInputHandler backgroundInputHandler;

    public static SharedObjects Instance;

    void OnEnable()
    {
        Instance = this;
    }
}
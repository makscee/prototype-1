using UnityEngine;

public class SharedObjects : MonoBehaviour
{
    public GameObject BackCanvas;
    public GameObject MidCanvas;
    public GameObject FrontCanvas;
    public Camera Camera;

    public static SharedObjects Instance;

    void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        Instance = this;
    }
}
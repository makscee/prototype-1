using UnityEngine;

public class SharedObjects : MonoBehaviour
{
    public GameObject Canvas;
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
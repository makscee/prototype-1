using UnityEngine;

public class SharedObjects : MonoBehaviour
{
    public GameObject Canvas;

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
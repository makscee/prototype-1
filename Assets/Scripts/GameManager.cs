using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    void OnEnable()
    {
        Instance = this;
        var cam = SharedObjects.Instance.Camera;
        ColorPalette.SubscribeToPalette(cam.gameObject, (c) => cam.backgroundColor = c, 0);
    }
    void OnDisable()
    {
        //ColorPalette.ClearListeners();
    }

    void Update()
    {
        ColorPalette.Update();
        GlobalPulse.Update();
    }
}

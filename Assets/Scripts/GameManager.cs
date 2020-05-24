using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    void OnEnable()
    {
        Instance = this;
        ColorPalette.SubscribeToPalette((c) => Camera.main.backgroundColor = c, 0);
    }
    void OnDisable()
    {
        ColorPalette.ClearListeners();
    }

    void Update()
    {
        ColorPalette.Update();
    }
}

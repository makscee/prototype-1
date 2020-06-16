using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static bool BlockClicked;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // if (MouseBind.IsBound())
            // {
            //     MouseBind.Get().UnbindAll();
            // }
            // else
            // {
            //     var b = Block.Create();
            //     b.Bind(MouseBind.Get(), Vector2.zero, 1);
            // }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            // ColorPalette.SwitchToNextPalette();
            // ColorPalette.AnimateSwitchToNextPalette();
        }
    }
}
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public TetrisField field;
    public Camera camera;
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

        if (Input.GetMouseButtonDown(2))
        {
            var b = Block.Create();
            b.transform.position += new Vector3(200, 200);
            // Debug.Log(
            //     RectTransformUtility.ScreenPointToLocalPointInRectangle(field.RectTransform, Input.mousePosition, null, out var v));
            // Debug.Log(v);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            // ColorPalette.SwitchToNextPalette();
            ColorPalette.AnimateSwitchToNextPalette();
        }
    }
    
    
}
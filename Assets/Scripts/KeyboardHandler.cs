using UnityEngine;

public class KeyboardHandler : MonoBehaviour
{
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
            GameManager.Instance.LoadSavedState();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameManager.Instance.ClearField();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.SaveGameToFile();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.LoadGameFromFile();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log(GameSerialized.Create().ToJson());
        }
    }
}
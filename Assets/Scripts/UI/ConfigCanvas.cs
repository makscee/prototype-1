using UnityEngine;
using UnityEngine.UI;

public class ConfigCanvas : MonoBehaviour
{
    CanvasScaler _canvasScaler;

    void Awake()
    {
        _canvasScaler = GetComponent<CanvasScaler>();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
    
    public Vector2 ScaledScreenPos(Vector2 v)
    {
        var scale = _canvasScaler.referenceResolution / new Vector2(Screen.width, Screen.height);
        return v * scale;
    }
}
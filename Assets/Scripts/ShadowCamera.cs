using System;
using UnityEngine;

public class ShadowCamera : MonoBehaviour
{
    public Camera mainCamera;
    public Camera shadowCamera;
    public RenderTexture renderTexture;

    public static ShadowCamera Instance;
    void OnEnable()
    {
        if (renderTexture == null)
        {
            var desc = new RenderTextureDescriptor(Screen.width, Screen.height);
            renderTexture = new RenderTexture(desc);
            shadowCamera.targetTexture = renderTexture;
        }
        Instance = this;
    }

    void Update()
    {
        transform.position = mainCamera.transform.position;
        shadowCamera.orthographicSize = mainCamera.orthographicSize;
    }
}
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
        renderTexture.width = mainCamera.pixelWidth;
        renderTexture.height = mainCamera.pixelHeight;
        Instance = this;
    }

    void Update()
    {
        transform.position = mainCamera.transform.position;
        shadowCamera.orthographicSize = mainCamera.orthographicSize;
    }
}
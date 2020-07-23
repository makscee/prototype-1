using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class NoiseGenerator : MonoBehaviour
{
    public Material material;
    public CustomRenderTexture renderTexture;
    static NoiseGenerator _instance;
    Renderer _renderer;

    void OnEnable()
    {
        _instance = this;
        renderTexture.Release();
        renderTexture.width = Screen.width;
        renderTexture.height = Screen.height;
        renderTexture.Initialize();
        // _renderer = GetComponent<Renderer>();
        // renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        // renderTexture.Create();
        // RenderPipelineManager.endFrameRendering += BlitTexture;
    }

    void Update()
    {
    }


    void BlitTexture(ScriptableRenderContext context, Camera[] cameras)
    {
        // var prev = RenderTexture.active;
        // RenderTexture.active = renderTexture;
        // Graphics.Blit(null, renderTexture, material);
        // RenderTexture.active = prev;
    }

    public static RenderTexture GetTexture()
    {
        return _instance.renderTexture;
    }
}
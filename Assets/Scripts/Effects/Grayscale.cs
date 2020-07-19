using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(GrayscaleRenderer), PostProcessEvent.AfterStack, "Custom/Grayscale")]
public sealed class Grayscale : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Grayscale effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
    
    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        return enabled.value
               && blend.value > 0f;
    }
}
 
public sealed class GrayscaleRenderer : PostProcessEffectRenderer<Grayscale>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Grayscale"));
        sheet.properties.SetFloat("_Blend", settings.blend);
        if (ShadowCamera.Instance != null)
            sheet.properties.SetTexture("_Mask", ShadowCamera.Instance.renderTexture);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
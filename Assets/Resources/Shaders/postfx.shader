Shader "Hidden/Custom/PostFx"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_Mask, sampler_Mask);
        TEXTURE2D_SAMPLER2D(_Noise, sampler_Noise);
        float _Blend;
        float _Offset;
        float4 _ColorPrimary;
        float4 _ColorSecondary;
        float4 _ColorPass;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            float4 maskColor = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord);
            float noise = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, i.texcoord);
            //float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
            //color.rgb = lerp(color.rgb, luminance.xxx, _Blend.xxx);
            float minSquare = min(_ScreenParams.x, _ScreenParams.y);
            float2 coords;
            coords.x = _ScreenParams.x / minSquare * i.texcoordStereo.x;
            coords.y = _ScreenParams.y / minSquare * i.texcoordStereo.y;
            
            coords.x += _Offset;
            coords.y += _Offset / 4;
            //if (maskColor.r == 1)
            //{
            //    if (noise > 0.8) 
            //    {
            //        color = _ColorPrimary;
            //    } else 
            //    {
            //        color = _ColorSecondary * (1 - noise);
            //    }
            //}
            
            color = color + (_ColorPrimary * noise - color) * maskColor.r;
            color = color + (_ColorSecondary - color) * maskColor.g;
            return color;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}
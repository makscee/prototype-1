Shader "Hidden/Custom/Grayscale"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_Mask, sampler_Mask);
        float _Blend;
        float _Offset;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            float4 maskColor = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord);
            //float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
            //color.rgb = lerp(color.rgb, luminance.xxx, _Blend.xxx);
            float minSquare = min(_ScreenParams.x, _ScreenParams.y);
            float2 coords;
            coords.x = _ScreenParams.x / minSquare * i.texcoordStereo.x;
            coords.y = _ScreenParams.y / minSquare * i.texcoordStereo.y;
            if (maskColor.r == 1) 
            {
                color.rgb = 0;
                coords.x += _Offset;
                coords.y += _Offset / 4;
                if (floor(coords.x * 100) % 10 == 0 && floor(coords.y * 100) % 10 == 0) color.rgb = 1;
            }
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
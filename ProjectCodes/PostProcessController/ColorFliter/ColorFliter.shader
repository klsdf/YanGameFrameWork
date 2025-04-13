Shader "CustomEffects/ColorFliter"
{
    Properties
    {
        _Intensity ("Intensity", Float) = 0.5
        _Color ("Color", Color) = (1,0,0,1)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off
        Cull Off
        Pass
        {
            Name "ColorFliter"
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            float _Intensity;
            float4 _Color;
            
            
            float4 Frag(Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                return lerp(color, _Color, _Intensity);
            }
            ENDHLSL
        }
    }
}
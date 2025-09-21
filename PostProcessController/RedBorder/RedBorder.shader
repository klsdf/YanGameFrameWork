Shader "Custom/RedBorder"
{
    Properties
    {
        _BorderColor ("Border Color", Color) = (1, 0, 0, 1)
        _BorderWidth ("Border Width", Range(0.0, 0.5)) = 0.1
        _BorderIntensity ("Border Intensity", Range(0.0, 1.0)) = 1.0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
        }
        
        LOD 100
        ZWrite Off
        Cull Off
        
        Pass
        {
            Name "RedBorderPass"
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            float4 _BorderColor;
            float _BorderWidth;
            float _BorderIntensity;
            
            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float4 originalColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);
                
                // 计算到屏幕边缘的距离
                float2 distanceToEdge = min(uv, 1.0 - uv);
                float minDistance = min(distanceToEdge.x, distanceToEdge.y);
                
                // 计算边框强度
                float borderFactor = smoothstep(_BorderWidth, 0.0, minDistance);
                
                // 混合原始颜色和边框颜色
                float4 borderColor = _BorderColor * _BorderIntensity;
                float4 finalColor = lerp(originalColor, borderColor, borderFactor);
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}

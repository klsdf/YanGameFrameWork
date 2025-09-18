Shader "CustomEffects/UnderSeaDisturb"
{
    Properties
    {
        _BlueTintColor ("Blue Tint Color (RGBA, A = intensity)", Color) = (0.4, 0.6, 1.0, 0.25)
        _RippleAmount ("Ripple Amount", Float) = 0.02
        _RippleFrequency ("Ripple Frequency", Float) = 10.0
        _RippleSpeed ("Ripple Speed", Float) = 2.0
        _NoiseScale ("Noise Scale", Float) = 3.0
        _NoiseThreshold ("Noise Threshold", Range(0,1)) = 0.7
        _BubbleIntensity ("Bubble Intensity", Float) = 0.04
    }
    
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off
        Cull Off
        Pass
        {
            Name "UnderSeaDisturb"
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            float4 _BlueTintColor;
            float _RippleAmount;
            float _RippleFrequency;
            float _RippleSpeed;
            float _NoiseScale;
            float _NoiseThreshold;
            float _BubbleIntensity;
            
            // Simple hash-based value noise (fast, non-periodic)
            float2 hash22(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.xx + p3.yz) * p3.zy);
            }
            
            float noise2d(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float2 a = hash22(i + float2(0,0));
                float2 b = hash22(i + float2(1,0));
                float2 c = hash22(i + float2(0,1));
                float2 d = hash22(i + float2(1,1));
                float n0 = lerp(a.x, b.x, u.x);
                float n1 = lerp(c.x, d.x, u.x);
                return lerp(n0, n1, u.y);
            }
            
            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float t = _Time.y * _RippleSpeed;
                
                // Base gentle ripples
                float waveX = sin(uv.y * _RippleFrequency + t);
                float waveY = cos(uv.x * (_RippleFrequency * 0.8) + t * 1.1);
                float2 baseOffset = float2(waveX, waveY * 0.75) * _RippleAmount;
                
                // Noise-masked local "bubble" distortion
                float2 nUv = uv * _NoiseScale + float2(0.0, t * 0.25);
                float n = noise2d(nUv);
                float mask = smoothstep(_NoiseThreshold, _NoiseThreshold + 0.1, n);
                
                // Bubble adds a small radial-like push based on noise gradient approximation
                float eps = 0.002;
                float nx = noise2d(nUv + float2(eps, 0.0)) - n;
                float ny = noise2d(nUv + float2(0.0, eps)) - n;
                float2 grad = normalize(float2(nx, ny) + 1e-5);
                float2 bubbleOffset = grad * _BubbleIntensity * mask;
                
                float2 offset = baseOffset + bubbleOffset;
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + offset);
                
                // Blue tint using alpha as intensity
                color = lerp(color, float4(_BlueTintColor.rgb, 1.0), saturate(_BlueTintColor.a));
                return color;
            }
            ENDHLSL
        }
    }
}
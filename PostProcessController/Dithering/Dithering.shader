Shader "Custom/Dithering"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _PaletteColorCount ("Mixed Color Count", float) = 4
        _PaletteHeight ("Palette Height", float) = 128
        _PaletteTex ("Palette", 2D) = "black" {}
        _PatternSize ("Pattern Size", float) = 8
        _PatternTex ("Pattern Texture", 2D) = "gray" {}
        _PatternScale("Pattern Scale", float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 200

        Pass
        {
            Name "DitheringEffect"
            Blend Off
            Cull Off
            ZTest Always
            ZWrite Off

            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                TEXTURE2D(_PaletteTex);
                SAMPLER(sampler_PaletteTex);
                TEXTURE2D(_PatternTex);
                SAMPLER(sampler_PatternTex);

                float _PaletteColorCount;
                float _PaletteHeight;
                float _PatternSize;
                float _PatternScale;

                struct VertexInput
                {
                    float4 position : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct FragmentInput
                {
                    float4 position : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float4 ditherPos : TEXCOORD1;
                };

                half4 GetDitherPos(float4 vertex, float ditherSize)
                {
                    // 使用 TransformObjectToHClip 将顶点转换到裁剪空间
                    float4 clipPos = TransformObjectToHClip(vertex.xyz);
                    float2 screenPos = clipPos.xy / clipPos.w * 0.5 + 0.5;
                    // 使用屏幕参数计算抖动像素位置
                    return half4(screenPos * _ScreenParams.xy / ditherSize, 0, clipPos.w);
                }

                FragmentInput vert(VertexInput v)
                {
                    FragmentInput o;
                    o.position = TransformObjectToHClip(v.position.xyz);
                    // o.uv = v.uv;
                    o.uv = float2(v.uv.x, 1 - v.uv.y);
                    o.ditherPos = GetDitherPos(v.position, _PatternSize);
                    return o;
                }

                half3 GetDitherColor(half3 color, TEXTURE2D(ditherTex), SAMPLER(sampler_ditherTex),
                                    TEXTURE2D(paletteTex), SAMPLER(sampler_paletteTex),
                                    float paletteHeight, float4 ditherPos, float colorCount, float patternScale)
                {
                    // 获取当前像素的抖动值，使用 ditherPos 和 patternScale 计算纹理坐标
                    half ditherValue = SAMPLE_TEXTURE2D(ditherTex, sampler_ditherTex, (ditherPos.xy / ditherPos.w) * patternScale).r;
                    ditherValue = min(ditherValue, 0.99);

                    // 计算调色板纹理坐标
                    float u = min(floor(color.r * 16), 15) / 16 + clamp(color.b * 16, 0.5, 15.5) / 256;
                    float v = (clamp(color.g * 16, 0.5, 15.5) + floor(ditherValue * colorCount) * 16) / paletteHeight;

                    // 返回调色板纹理中的新颜色
                    return SAMPLE_TEXTURE2D(paletteTex, sampler_paletteTex, float2(u, v)).rgb;
                }

                half4 frag(FragmentInput i) : SV_Target
                {
                    half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                    half3 ditheredColor = GetDitherColor(c.rgb, _PatternTex, sampler_PatternTex, _PaletteTex, sampler_PaletteTex, _PaletteHeight, i.ditherPos, _PaletteColorCount, _PatternScale);
                    return half4(ditheredColor, c.a);
                }
            ENDHLSL
        }
    }

    Fallback "Unlit/Texture"
}
Shader "YanGameFrameWork/Dithering" 
{
    Properties 
    {
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
                #pragma vertex Vert
                #pragma fragment Frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
                

                float _PaletteColorCount;
                TEXTURE2D(_PaletteTex);
                SAMPLER(sampler_PaletteTex);
                float _PaletteHeight;
                float _PatternSize;
                float _PatternScale;
                TEXTURE2D(_PatternTex);
                SAMPLER(sampler_PatternTex);
          
    

           



                // /// <summary>
                // /// 计算输入颜色应映射到的色阶索引，并结合抖动值做微调
                // /// </summary>
                // /// <param name="color">原始颜色</param>
                // /// <param name="ditherValue">抖动值</param>
                // /// <returns>映射后的颜色</returns>
                // float3 DitherMapColor(float3 color, float ditherValue)
                // {
                //     // 以亮度为基准分段
                //     float luminance = dot(color, float3(0.299, 0.587, 0.114));
                //     float step = 1.0 / _DitherColorCount;
                //     // 抖动偏移
                //     float dithered = saturate(luminance + (ditherValue - 0.5) * step);
                //     int idx = (int)floor(dithered * _DitherColorCount);
                //     idx = clamp(idx, 0, (int)_DitherColorCount - 1);
                //     return GetDitherColorByIndex(idx).rgb;
                // }



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

                half4 GetDitherPos(float4 vertex, float ditherSize)
                {
                    // 使用 TransformObjectToHClip 将顶点转换到裁剪空间
                    float4 clipPos = TransformObjectToHClip(vertex.xyz);
                    float2 screenPos = clipPos.xy / clipPos.w * 0.5 + 0.5;
                    // 使用屏幕参数计算抖动像素位置
                    return half4(screenPos * _ScreenParams.xy / ditherSize, 0, clipPos.w);
                }

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

                FragmentInput vert(VertexInput v)
                {
                    FragmentInput o;
                    o.position = TransformObjectToHClip(v.position.xyz);
                    // o.uv = v.uv;
                    o.uv = float2(v.uv.x, 1 - v.uv.y);
                    o.ditherPos = GetDitherPos(v.position, _PatternSize);
                    return o;
                }


                float getUV(float2 uv)
                {
                    return float2(uv.x, 1 - uv.y);
                }


                
               

                /// <summary>
                /// 片元着色器，进行抖动映射
                /// </summary>
                half4 Frag(Varyings i) : SV_Target
                {
                    // 采样主纹理
                    half4 c = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp,i.texcoord);
                    
                    // 使用texcoord计算屏幕位置，参考GetDitherPos函数的逻辑
                    // texcoord已经是0-1范围的屏幕坐标，直接乘以屏幕参数
                    float2 screenPos = i.texcoord * _ScreenParams.xy;
                    // 除以抖动大小，并保持与原始函数一致的结构
                    float4 ditherPos = float4(screenPos / _PatternSize, 0, 1);
                    
                    half3 ditheredColor = GetDitherColor(c.rgb, _PatternTex, sampler_PatternTex, 
                         _PaletteTex, sampler_PaletteTex, _PaletteHeight, 
                         ditherPos, _PaletteColorCount, _PatternScale);
                    return half4(ditheredColor, c.a);
                }
            ENDHLSL
        }
    }

    Fallback "Unlit/Texture"
}
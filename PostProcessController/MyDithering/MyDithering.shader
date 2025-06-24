Shader "YanGameFrameWork/Dithering" 
{
    Properties 
    {
        _PatternTex ("Pattern Texture", 2D) = "white" {}
        _PatternScale("Pattern Scale", float) = 1
        _DitherColorCount ("Dither Color Count", float) = 4
        // 颜色数组，最多支持8种颜色
        _DitherColors0 ("Dither Color 0", Color) = (1,1,1,1)
        _DitherColors1 ("Dither Color 1", Color) = (0,0,0,1)
        _DitherColors2 ("Dither Color 2", Color) = (1,0,0,1)
        _DitherColors3 ("Dither Color 3", Color) = (0,1,0,1)
        _DitherColors4 ("Dither Color 4", Color) = (0,0,1,1)
        _DitherColors5 ("Dither Color 5", Color) = (1,1,0,1)
        _DitherColors6 ("Dither Color 6", Color) = (0,1,1,1)
        _DitherColors7 ("Dither Color 7", Color) = (1,0,1,1)
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
                

                TEXTURE2D(_PatternTex);
                SAMPLER(sampler_PatternTex);

                float _PatternScale;
                float _DitherColorCount;
                float4 _DitherColors0;
                float4 _DitherColors1;
                float4 _DitherColors2;
                float4 _DitherColors3;
                float4 _DitherColors4;
                float4 _DitherColors5;
                float4 _DitherColors6;
                float4 _DitherColors7;

                /// <summary>
                /// 获取颜色数组中的指定索引颜色
                /// </summary>
                /// <param name="idx">颜色索引</param>
                /// <returns>对应的颜色</returns>
                float4 GetDitherColorByIndex(int idx)
                {
                    // 由于HLSL不支持动态数组，这里用if-else实现
                    if(idx == 0) return _DitherColors0;
                    else if(idx == 1) return _DitherColors1;
                    else if(idx == 2) return _DitherColors2;
                    else if(idx == 3) return _DitherColors3;
                    else if(idx == 4) return _DitherColors4;
                    else if(idx == 5) return _DitherColors5;
                    else if(idx == 6) return _DitherColors6;
                    else if(idx == 7) return _DitherColors7;
                    return float4(0,0,0,1);
                }



                /// <summary>
                /// 计算输入颜色应映射到的色阶索引，并结合抖动值做微调
                /// </summary>
                /// <param name="color">原始颜色</param>
                /// <param name="ditherValue">抖动值</param>
                /// <returns>映射后的颜色</returns>
                float3 DitherMapColor(float3 color, float ditherValue)
                {
                    // 以亮度为基准分段
                    float luminance = dot(color, float3(0.299, 0.587, 0.114));
                    float step = 1.0 / _DitherColorCount;
                    // 抖动偏移
                    float dithered = saturate(luminance + (ditherValue - 0.5) * step);
                    int idx = (int)floor(dithered * _DitherColorCount);
                    idx = clamp(idx, 0, (int)_DitherColorCount - 1);
                    return GetDitherColorByIndex(idx).rgb;
                }

                /// <summary>
                /// 片元着色器，进行抖动映射
                /// </summary>
                half4 Frag(Varyings i) : SV_Target
                {
                    // 采样主纹理
                    half4 c = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, i.texcoord);
                    // 采样Pattern纹理，取R通道作为抖动值
                    float ditherValue = SAMPLE_TEXTURE2D(_PatternTex, sampler_PatternTex, i.texcoord * _PatternScale).r;
                    float3 ditheredColor = DitherMapColor(c.rgb, ditherValue);
                    return half4(ditheredColor, c.a);
                }
            ENDHLSL
        }
    }

    Fallback "Unlit/Texture"
}
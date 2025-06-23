Shader "Custom/PixelationPostProcess"
{
    Properties
    {
        _PixelSize ("Pixel Size", Range(1, 100)) = 8
        _MainTex ("Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Pixelation"
            ZWrite Off 
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // #include "UnityCG.cginc"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            // sampler2D _MainTex;
            float _PixelSize;

            Varyings vert(Attributes input)
            {
                Varyings output;
                // output.positionCS = UnityObjectToClipPos(input.positionOS.xyz);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
#if UNITY_REVERSED_Z
                output.uv = float2(input.uv.x, 1 - input.uv.y);
#else
                output.uv = input.uv;
#endif
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                //想象一个棋盘  texelSize * _PixelSize 决定每个格子的大小
                // 获取纹理的像素大小信息
                float2 texelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                
                // 计算像素化后的UV坐标
                float2 pixelatedUV = round(input.uv / (texelSize * _PixelSize)) * (texelSize * _PixelSize);
                
                // 采样像素化后的颜色
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pixelatedUV);
                // half4 color = tex2D(_MainTex,input.uv);
                
                return color;
            }
            ENDHLSL
        }
    }
}
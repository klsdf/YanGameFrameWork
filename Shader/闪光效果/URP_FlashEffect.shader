Shader "Custom/URP_FlashEffect"
{
    Properties
    {
        _MainTex ("主纹理", 2D) = "white" {}
        
        _MaskTex ("遮罩纹理", 2D) = "white" {}
        _MaskTex_ST ("遮罩纹理ST", Vector) = (1,1,0,0)
        
        _FlashTex ("闪光纹理", 2D) = "white" {}
        _FlashColor ("闪光颜色", Color) = (1,1,1,1)
        _FlashIntensity ("闪光强度", Range(0,5)) = 1
        _FlashOffset ("灰度图偏移", Range(-1,1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };


            sampler2D _MaskTex;
            sampler2D _FlashTex;
            float4 _MaskTex_ST;

            float4 _FlashTex_ST;
            float4 _FlashColor;
            float _FlashIntensity;
            float _FlashOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
               // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
     
                half4 mask = tex2D(_MaskTex, i.uv);

                float2 flashUV = i.uv;
                flashUV.x = flashUV.x + _FlashOffset; // 横向循环滚动
                half4 flash = tex2D(_FlashTex, flashUV) * _FlashColor * _FlashIntensity;

                flash *= mask.a;

                half4 final = flash;
                final.a = saturate(flash.a * _FlashIntensity);

                return final;
            }
            ENDHLSL
        }
    }
}

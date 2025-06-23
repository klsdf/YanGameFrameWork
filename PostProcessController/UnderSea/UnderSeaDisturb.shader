Shader "Custom/UnderSeaDisturb_Builtin"
{
    Properties
    {
        _MainTex ("主纹理", 2D) = "white" {}
        _DisturbStrength ("扰动强度", Range(0,0.1)) = 0.03
        _DisturbFrequency ("扰动频率", Range(1,20)) = 8
        _DisturbSpeed ("扰动速度", Range(0,5)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _DisturbStrength;
            float _DisturbFrequency;
            float _DisturbSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            /// <summary>
            /// 顶点着色器，传递屏幕UV
            /// </summary>
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            /// <summary>
            /// 片元着色器，计算扰动并采样主纹理
            /// </summary>
            fixed4 frag (v2f i) : SV_Target
            {
                // 计算扰动
                float wave = sin(i.uv.y * _DisturbFrequency + _Time.x * _DisturbSpeed) * _DisturbStrength;
                float2 disturbedUV = i.uv + float2(wave, 0);

                // 采样主纹理
                fixed4 col = tex2D(_MainTex, disturbedUV);
                return col;
            }
            ENDCG
        }
    }
    FallBack Off
} 
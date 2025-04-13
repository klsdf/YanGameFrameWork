Shader "Custom/Test"
{
    Properties
    {
        _Color("Color", Color) = (1,0,0,1)
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent"
            "Queue"="Transparent" 
        }

        Pass
        {
            Name "Test"
            ZTest Always 
            Cull Off 
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha  // 添加混合模式
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // o.uv = v.vertex.xy * 0.5 + 0.5;
                return o;
            }

            sampler2D _MainTex;
            float4 _Color;
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                return lerp(color, _Color, _Color.a);
            }
            ENDHLSL
        }
    }
} 
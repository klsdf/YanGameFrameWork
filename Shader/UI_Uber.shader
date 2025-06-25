Shader "YanGF/UI_Uber"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ClipTopY ("Y的上方的裁剪", Range(0,1)) = 0.5
        _ClipBottomY ("Y的底部的裁剪", Range(0,1)) = 0.0
        _Color ("颜色", Color) = (1,1,1,1)
        _Radius ("圆角的半径", Range(0,0.5)) = 0.1
        _ClipLeftX ("X的左侧裁剪", Range(0,1)) = 0.0
        _ClipRightX ("X的右侧裁剪", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Lighting Off Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _ClipTopY;
            float _ClipBottomY;
            float _Radius;
            float _ClipLeftX;
            float _ClipRightX;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (i.uv.x < _ClipLeftX || i.uv.x > (1.0 - _ClipRightX) || i.uv.y < _ClipBottomY || i.uv.y > (1.0 - _ClipTopY))
                    discard;

                float2 uv = i.uv;
                float2 leftBottom = float2(_ClipLeftX, _ClipBottomY);
                float2 rightBottom = float2(1.0 - _ClipRightX, _ClipBottomY);
                float2 leftTop = float2(_ClipLeftX, 1.0 - _ClipTopY);
                float2 rightTop = float2(1.0 - _ClipRightX, 1.0 - _ClipTopY);
                float r = _Radius;

                // 左下
                if (uv.x < (_ClipLeftX + r) && uv.y < (_ClipBottomY + r) && distance(uv, leftBottom + float2(r, r)) > r)
                    discard;
                // 右下
                if (uv.x > (1.0 - _ClipRightX - r) && uv.y < (_ClipBottomY + r) && distance(uv, rightBottom + float2(-r, r)) > r)
                    discard;
                // 左上
                if (uv.x < (_ClipLeftX + r) && uv.y > (1.0 - _ClipTopY - r) && distance(uv, leftTop + float2(r, -r)) > r)
                    discard;
                // 右上
                if (uv.x > (1.0 - _ClipRightX - r) && uv.y > (1.0 - _ClipTopY - r) && distance(uv, rightTop + float2(-r, -r)) > r)
                    discard;
         

                fixed4 col = tex2D(_MainTex, uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
}

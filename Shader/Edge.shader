Shader "Custom/2DHighlightEdge"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _InnerEdgeColor ("Inner Edge Color", Color) = (1,0,0,1)
        _InnerEdgeStrength ("Inner Edge Strength", Float) = 2.0
        _OuterEdgeColor ("Outer Edge Color", Color) = (0,1,0,1)
        _OuterEdgeStrength ("Outer Edge Strength", Float) = 1.0
        _OuterEdgeThickness ("Outer Edge Thickness", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _InnerEdgeColor;
            float _InnerEdgeStrength;
            float4 _OuterEdgeColor;
            float _OuterEdgeStrength;
            float _OuterEdgeThickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float luminance(float3 color)
            {
                return dot(color, float3(0.299, 0.587, 0.114));
            }

            float GetAlpha(float2 uv)
            {
                return tex2D(_MainTex, uv).a;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 texel = _MainTex_TexelSize.xy;

                // 内边缘检测（Sobel）
                float3 col00 = tex2D(_MainTex, uv + texel * float2(-1, -1)).rgb;
                float3 col01 = tex2D(_MainTex, uv + texel * float2(-1, 0)).rgb;
                float3 col02 = tex2D(_MainTex, uv + texel * float2(-1, 1)).rgb;
                float3 col10 = tex2D(_MainTex, uv + texel * float2(0, -1)).rgb;
                float3 col12 = tex2D(_MainTex, uv + texel * float2(0, 1)).rgb;
                float3 col20 = tex2D(_MainTex, uv + texel * float2(1, -1)).rgb;
                float3 col21 = tex2D(_MainTex, uv + texel * float2(1, 0)).rgb;
                float3 col22 = tex2D(_MainTex, uv + texel * float2(1, 1)).rgb;

                float lum00 = luminance(col00);
                float lum01 = luminance(col01);
                float lum02 = luminance(col02);
                float lum10 = luminance(col10);
                float lum12 = luminance(col12);
                float lum20 = luminance(col20);
                float lum21 = luminance(col21);
                float lum22 = luminance(col22);

                float gx = -lum00 - 2.0 * lum01 - lum02 + lum20 + 2.0 * lum21 + lum22;
                float gy = -lum00 - 2.0 * lum10 - lum20 + lum02 + 2.0 * lum12 + lum22;

                float innerEdge = sqrt(gx * gx + gy * gy) * _InnerEdgeStrength;
                float4 innerEdgeColor = _InnerEdgeColor * saturate(innerEdge);

                // 外边缘检测（alpha变化，支持宽度）
                float alpha = GetAlpha(uv);
                float2 offset = _OuterEdgeThickness * texel;

                float outerEdge = 0.0;
                outerEdge += abs(alpha - GetAlpha(uv + float2(offset.x, 0)));
                outerEdge += abs(alpha - GetAlpha(uv + float2(-offset.x, 0)));
                outerEdge += abs(alpha - GetAlpha(uv + float2(0, offset.y)));
                outerEdge += abs(alpha - GetAlpha(uv + float2(0, -offset.y)));
                outerEdge = saturate(outerEdge * _OuterEdgeStrength);

                float4 texColor = tex2D(_MainTex, uv);
                float4 outerEdgeColor = _OuterEdgeColor * outerEdge * (1.0 - texColor.a);

                // 叠加
                return texColor + innerEdgeColor * texColor.a + outerEdgeColor;
            }
            ENDCG
        }
    }
}

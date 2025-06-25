Shader "Custom/2DHighlightEdge"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (1,0,0,1)
        _EdgeWidth ("Edge Width (px)", Float) = 1.0
        _AlphaThreshold ("Alpha Threshold", Range(0,1)) = 0.1
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
            float4 _EdgeColor;
            float _EdgeWidth;
            float _AlphaThreshold;

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

            #define MAX_EDGE_WIDTH 99

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 texel = _MainTex_TexelSize.xy;
                float4 texColor = tex2D(_MainTex, uv);

                float alpha = texColor.a;
                float threshold = _AlphaThreshold;
                int edgeWidth = int(_EdgeWidth + 0.5);

                bool isEdge = false;

                if (alpha > threshold)
                {
                    float2 dirs[8] = {
                        float2(1,0), float2(-1,0), float2(0,1), float2(0,-1),
                        float2(1,1), float2(-1,1), float2(1,-1), float2(-1,-1)
                    };

                    for (int s = 1; s <= MAX_EDGE_WIDTH; s++)
                    {
                        if (s > edgeWidth) break;
                        for (int d = 0; d < 8; d++)
                        {
                            float2 offset = dirs[d] * texel * s;
                            float a = tex2D(_MainTex, uv + offset).a;
                            if (a < threshold)
                            {
                                isEdge = true;
                                break;
                            }
                        }
                        if (isEdge) break;
                    }
                }

                float4 edgeColor = _EdgeColor;
                float4 result = isEdge ? edgeColor : texColor;
                result.a = texColor.a;

                return result;
            }
            ENDCG
        }
    }
}

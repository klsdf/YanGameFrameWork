Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineSize;
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // 如果当前像素透明，检查周围像素
                if (c.a == 0) {
                    // 使用固定的采样点而不是循环
                    float2 pixelSize = _MainTex_TexelSize.xy * _OutlineSize;
                    
                    fixed up = tex2D(_MainTex, IN.texcoord + float2(0, pixelSize.y)).a;
                    fixed down = tex2D(_MainTex, IN.texcoord + float2(0, -pixelSize.y)).a;
                    fixed left = tex2D(_MainTex, IN.texcoord + float2(-pixelSize.x, 0)).a;
                    fixed right = tex2D(_MainTex, IN.texcoord + float2(pixelSize.x, 0)).a;
                    
                    // 也可以添加对角线检测
                    fixed upLeft = tex2D(_MainTex, IN.texcoord + float2(-pixelSize.x, pixelSize.y)).a;
                    fixed upRight = tex2D(_MainTex, IN.texcoord + float2(pixelSize.x, pixelSize.y)).a;
                    fixed downLeft = tex2D(_MainTex, IN.texcoord + float2(-pixelSize.x, -pixelSize.y)).a;
                    fixed downRight = tex2D(_MainTex, IN.texcoord + float2(pixelSize.x, -pixelSize.y)).a;
                    
                    if (up > 0 || down > 0 || left > 0 || right > 0 || 
                        upLeft > 0 || upRight > 0 || downLeft > 0 || downRight > 0) {
                        return _OutlineColor;
                    }
                }
                
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
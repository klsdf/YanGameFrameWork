Shader "Custom/MaskTransition"
{
    Properties
    {

        
        /// <summary>
        /// 主纹理
        /// </summary>
        _MainTex ("主纹理", 2D) = "white" {}
        
        /// <summary>
        /// 遮罩灰度图
        /// </summary>
        _MaskTex ("遮罩纹理", 2D) = "white" {}
        
        /// <summary>
        /// 遮罩阈值，控制转场效果
        /// </summary>
        _MaskThreshold ("遮罩阈值", Range(0, 1)) = 0.5
        
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            /// <summary>
            /// 顶点着色器输入结构体
            /// </summary>
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            /// <summary>
            /// 顶点着色器输出结构体
            /// </summary>
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            /// <summary>
            /// 主纹理
            /// </summary>
            sampler2D _MainTex;
            
            
            /// <summary>
            /// 遮罩纹理
            /// </summary>
            sampler2D _MaskTex;
            
            /// <summary>
            /// 遮罩阈值
            /// </summary>
            float _MaskThreshold;
            

            
            /// <summary>
            /// 顶点着色器
            /// </summary>
            /// <param name="v">顶点数据</param>
            /// <returns>处理后的顶点数据</returns>
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            /// <summary>
            /// 片段着色器
            /// </summary>
            /// <param name="i">片段数据</param>
            /// <returns>最终颜色</returns>
            fixed4 frag (v2f i) : SV_Target
            {
                // 采样主纹理
                fixed4 col1 = tex2D(_MainTex, i.uv);
                
                // 采样遮罩纹理并获取灰度值
                fixed4 maskColor = tex2D(_MaskTex, i.uv);
                float maskValue = maskColor.r;

                // float blendFactor = step(_MaskThreshold, maskValue);
                // fixed4 finalColor = lerp(col1, col2, blendFactor);

                fixed4 finalColor;
                
                // 特殊处理边界情况
                if (_MaskThreshold <= 0.0)
                {
                    // 阈值为0时，完全显示第二张图片
                    finalColor = (0,0,0,0);
                }
                else if (_MaskThreshold >= 1.0)
                {
                    // 阈值为1时，完全显示第一张图片
                    finalColor = col1;
                }
                else
                {

                    if (maskValue <= _MaskThreshold)
                    {
                        finalColor = col1;
                    }
                    else
                    {
                        finalColor = (0,0,0,0);
                    }
                }
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    Fallback "Sprites/Default"
}

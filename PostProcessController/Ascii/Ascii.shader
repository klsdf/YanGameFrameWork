Shader "Post/Ascii_URP"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		[Toggle] _EnableAscii ("Enable Ascii", Float) = 1

		_Ascii ("Ascii Texture", 2D) = "white" {}

		_AsciiSplit ("Ascii Split", Float) = 18

		_Strength ("Strength", Range(1, 100)) = 10
	}
	SubShader
	{
		Tags 
		{ 
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
		}
		LOD 100
		ZWrite Off 
		Cull Off
		
		Pass
		{
			Name "Ascii"
			
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"


			// TEXTURE2D(_MainTex);
			// SAMPLER(sampler_MainTex);
			TEXTURE2D(_Ascii);
			SAMPLER(sampler_Ascii);
			
			CBUFFER_START(UnityPerMaterial)
				// float4 _MainTex_ST;
				float _EnableAscii;
				float _Strength;
				float _AsciiSplit;
			CBUFFER_END

			// Varyings vert(Attributes input)
			// {
			// 	Varyings output;
			// 	output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
			// 	output.uv = input.uv;
			// 	return output;
			// }

			float4 frag(Varyings input) : SV_Target
			{
				float u = input.texcoord.x;
				float v = input.texcoord.y;

				float width = 1 / _Strength;

				// 网格化UV坐标
				u = (1 / _Strength) * (int)(u / (1 / _Strength));
				v = (1 / _Strength) * (int)(v / (1 / _Strength));

				// 采样原始图像
				float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, float2(u, v));
				
				// 计算灰度值
				float grey = 1 - dot(color.rgb, float3(0.22, 0.707, 0.071));
				
				// 如果ASCII效果未启用，返回灰度图
				if (1 - _EnableAscii)
				{
					return float4(1 - grey.xxx, 1);
				}

				// 计算ASCII字符的UV偏移
				float u_offset = (input.texcoord.x - u) / width;
				float v_offset = (input.texcoord.y - v) / width;

				// 根据灰度值选择ASCII字符
				float g_index = round(grey * (_AsciiSplit - 1));
				float ascii_offset = (1 / _AsciiSplit) * g_index;

				// 计算最终的ASCII纹理UV坐标
				float2 ascii_uv = float2(
					ascii_offset + u_offset / _AsciiSplit - 0.0001, 
					v_offset
				);

				// 采样ASCII纹理
				float4 ascii_color = SAMPLE_TEXTURE2D(_Ascii, sampler_Ascii, ascii_uv);

				return ascii_color;
			}
			ENDHLSL
		}
	}
}
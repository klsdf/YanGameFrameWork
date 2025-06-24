using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 抖动图案类型枚举
/// </summary>
public enum PatternType
{
	Noise,  // 噪声图案
	Dots,   // 点阵图案
	Lines   // 线条图案
}

/// <summary>
/// 线条方向枚举
/// </summary>
public enum LineDirection
{
	Vertical,   // 垂直
	Horizontal, // 水平
	Slope45,    // 45度斜线
	Slope135    // 135度斜线
}

/// <summary>
/// 抖动图案配置资源类
/// 通过ScriptableObject实现，可以在Unity中创建和配置不同的抖动图案
/// </summary>
[Serializable]
[CreateAssetMenu(menuName = "Beffio/Dithering Pattern")]
public class Pattern : ScriptableObject 
{
	[Header("Pattern Settings")]
	public PatternType Type = PatternType.Noise; // 抖动图案类型
	public float MinimumValue = 0.0f;            // 图案最小值
	public float MaximumValue = 1.0f;            // 图案最大值

	public float ColorVariance = 0.1f;           // 颜色变化幅度
	public float ElementSize = 1.0f;             // 图案元素大小

	public LineDirection Direction = LineDirection.Horizontal; // 线条方向

	[Header("Texture Settings")]
	public int TextureSize = 8;                  // 生成纹理的尺寸
	public Texture2D Texture;                    // 图案纹理
	public bool HasTexture = false;              // 是否有自定义纹理

	public bool IsDirty = false;                 // 标记图案是否需要重新生成
}

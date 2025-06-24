using System;
using UnityEngine;

/// <summary>
/// 抖动后处理效果的参数设置类
/// </summary>
[Serializable]
public class MyDitheringSettings
{

    /// <summary>
    /// 抖动图案纹理
    /// </summary>
    public Texture2D patternTexture;

    /// <summary>
    /// 抖动图案缩放系数
    /// </summary>
    [Range(0.1f, 20f)]
    public float patternScale = 1f;

    /// <summary>
    /// 使用的抖动颜色数量（最大8）
    /// </summary>
    [Range(2, 8)]
    public int ditherColorCount = 4;

    /// <summary>
    /// 抖动颜色数组，最多8种颜色
    /// </summary>
    [Tooltip("最多支持8种颜色")]
    public Color[] ditherColors = new Color[4] { Color.white, Color.black, Color.red, Color.green };
}

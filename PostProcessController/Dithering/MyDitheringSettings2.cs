using System;
using UnityEngine;

/// <summary>
/// 与Dithering.shader参数一一对应的抖动后处理设置
/// </summary>
[Serializable]
public class MyDitheringSettings2
{
    /// <summary>
    /// 调色板颜色数量
    /// </summary>
    [Range(2, 8)]
    public int paletteColorCount = 4;

    /// <summary>
    /// 调色板高度
    /// </summary>
    [Range(1, 256)]
    public int paletteHeight = 128;

    /// <summary>
    /// 调色板纹理
    /// </summary>
    public Texture2D paletteTex;

    /// <summary>
    /// 抖动图案尺寸
    /// </summary>
    [Range(1, 64)]
    public float patternSize = 8f;

    /// <summary>
    /// 抖动图案纹理
    /// </summary>
    public Texture2D patternTex;

    /// <summary>
    /// 抖动图案缩放
    /// </summary>
    [Range(0.1f, 20f)]
    public float patternScale = 1f;
}

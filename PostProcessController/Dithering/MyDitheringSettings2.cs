using System;
using UnityEngine;

/// <summary>
/// 与Dithering2.shader参数一一对应的抖动后处理设置
/// </summary>
[Serializable]
public class MyDitheringSettings2
{
    /// <summary>
    /// 调色板颜色数量 (对应 _PaletteColorCount)
    /// </summary>
    [Range(1, 16)]
    public float paletteColorCount = 4f;

    /// <summary>
    /// 调色板高度 (对应 _PaletteHeight)
    /// </summary>
    [Range(1, 512)]
    public float paletteHeight = 128f;

    /// <summary>
    /// 调色板纹理 (对应 _PaletteTex)
    /// </summary>
    public Texture2D paletteTex;

    /// <summary>
    /// 抖动图案尺寸 (对应 _PatternSize)
    /// </summary>
    [Range(1, 64)]
    public float patternSize = 8f;

    /// <summary>
    /// 抖动图案纹理 (对应 _PatternTex)
    /// </summary>
    public Texture2D patternTex;

    /// <summary>
    /// 抖动图案缩放 (对应 _PatternScale)
    /// </summary>
    [Range(0.1f, 20f)]
    public float patternScale = 1f;
}

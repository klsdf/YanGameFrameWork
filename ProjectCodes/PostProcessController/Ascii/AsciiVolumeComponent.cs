using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 测试后处理效果的Volume组件
/// </summary>
public class AsciiVolumeComponent : VolumeComponent, IPostProcessComponent
{
    /// <summary>
    /// 效果的强度
    /// </summary>
    public FloatParameter intensity = new FloatParameter(1f);



    public TextureParameter asciiTexture = new TextureParameter(null);

    public bool IsActive() => intensity.value > 0f;

    public bool IsTileCompatible() => false;
} 
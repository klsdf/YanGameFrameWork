using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 颜色滤镜后处理效果的渲染特性
/// </summary>
public class ColorFliterRenderFeature : YanRenderFeature<ColorFliterRenderPass, RedTintSettings>
{
    /// <summary>
    /// 创建颜色滤镜渲染通道
    /// </summary>
    protected override ColorFliterRenderPass CreatePass(Material material, RedTintSettings settings)
    {
        var pass = new ColorFliterRenderPass(material, settings);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        return pass;
    }
}

[Serializable]
public class RedTintSettings
{
    /// <summary>
    /// 滤镜强度
    /// </summary>
    [Range(0, 1f)] public float intensity;
    /// <summary>
    /// 滤镜颜色
    /// </summary>
    public Color color;
}
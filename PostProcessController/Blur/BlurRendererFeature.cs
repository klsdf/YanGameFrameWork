using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 模糊后处理效果的渲染特性
/// </summary>
public class BlurRendererFeature : YanRenderFeature<BlurRenderPass, BlurSettings>
{
    /// <summary>
    /// 创建模糊渲染通道
    /// </summary>
    protected override BlurRenderPass CreatePass(Material material, BlurSettings settings)
    {
        var pass = new BlurRenderPass(material, settings);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        return pass;
    }
}

[Serializable]
public class BlurSettings
{
    [Range(0, 0.4f)] public float horizontalBlur;
    [Range(0, 0.4f)] public float verticalBlur;
}
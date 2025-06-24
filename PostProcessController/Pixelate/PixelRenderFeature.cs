using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 像素化后处理效果的渲染特性
/// </summary>
public class PixelRenderFeature : YanRenderFeature<PixelRenderPass, PixelSettings>
{
    /// <summary>
    /// 创建像素化渲染通道
    /// </summary>
    protected override PixelRenderPass CreatePass(Material material, PixelSettings settings)
    {
        var pass = new PixelRenderPass(material, settings);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        return pass;
    }
}

[System.Serializable]
public class PixelSettings
{
    /// <summary>
    /// 像素大小
    /// </summary>
    [Range(1, 50)] public int pixelSize = 8;
}



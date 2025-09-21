using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 红色边框后处理效果的渲染特性
/// </summary>
public class RedBorderRenderFeature : YanRenderFeature<RedBorderRenderPass, RedBorderSettings>
{
    /// <summary>
    /// 创建红色边框渲染通道
    /// </summary>
    protected override RedBorderRenderPass CreatePass(Material material, RedBorderSettings settings)
    {
        var pass = new RedBorderRenderPass(material, settings);
        pass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        return pass;
    }

    /// <summary>
    /// 设置边框颜色
    /// </summary>
    /// <param name="color">边框颜色</param>
    public void SetBorderColor(Color color)
    {
        if (settings != null)
        {
            settings.borderColor = color;
        }
    }

    /// <summary>
    /// 设置边框宽度
    /// </summary>
    /// <param name="width">边框宽度 (0-0.5)</param>
    public void SetBorderWidth(float width)
    {
        if (settings != null)
        {
            settings.borderWidth = Mathf.Clamp(width, 0.0f, 0.5f);
        }
    }

    /// <summary>
    /// 设置边框强度
    /// </summary>
    /// <param name="intensity">边框强度 (0-1)</param>
    public void SetBorderIntensity(float intensity)
    {
        if (settings != null)
        {
            settings.borderIntensity = Mathf.Clamp01(intensity);
        }
    }

    /// <summary>
    /// 启用或禁用红色边框效果
    /// </summary>
    /// <param name="enabled">是否启用</param>
    public void SetActive(bool enabled)
    {
        // 通过反射设置enabled属性，因为它是protected的
        var enabledField = typeof(ScriptableRendererFeature).GetField("m_Enabled", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        enabledField?.SetValue(this, enabled);
    }
}

[Serializable]
public class RedBorderSettings
{
    /// <summary>
    /// 边框颜色
    /// </summary>
    public Color borderColor = Color.red;
    
    /// <summary>
    /// 边框宽度
    /// </summary>
    [Range(0.0f, 0.5f)] public float borderWidth = 0.1f;
    
    /// <summary>
    /// 边框强度
    /// </summary>
    [Range(0.0f, 1.0f)] public float borderIntensity = 1.0f;
}
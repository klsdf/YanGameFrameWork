using UnityEngine.Rendering.Universal;
using UnityEngine;

/// <summary>
/// 渲染特性基类，提供材质管理功能
/// </summary>
public abstract class YanRenderFeature : ScriptableRendererFeature
{
    /// <summary>
    /// 渲染材质
    /// </summary>
    protected Material material;

    /// <summary>
    /// 设置材质并更新渲染通道
    /// </summary>
    /// <param name="material">要设置的新材质</param>
    public void SetMaterial(Material material)
    {
        this.material = material;
    }
}

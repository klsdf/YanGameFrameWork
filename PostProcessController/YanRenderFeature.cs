using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;

/// <summary>
/// 通用后处理特性基类，负责材质和通道的生命周期管理
/// </summary>
/// <typeparam name="TPass">渲染通道类型</typeparam>
/// <typeparam name="TSettings">设置类型</typeparam>
public abstract class YanRenderFeature<TPass, TSettings> : ScriptableRendererFeature
    where TPass : ScriptableRenderPass
{
    /// <summary>
    /// 使用的Shader
    /// </summary>
    [SerializeField] protected Shader shader;
    /// <summary>
    /// 材质实例
    /// </summary>
    protected Material material;
    /// <summary>
    /// 设置参数
    /// </summary>
    [SerializeField] protected TSettings settings;
    /// <summary>
    /// 渲染通道
    /// </summary>
    protected TPass pass;

    /// <summary>
    /// 创建渲染通道，由子类实现
    /// </summary>
    protected abstract TPass CreatePass(Material material, TSettings settings);

    /// <summary>
    /// 创建渲染通道
    /// </summary>
    public override void Create()
    {
        if (shader == null) return;
        material = new Material(shader);
        pass = CreatePass(material, settings);
        // 子类可在 CreatePass 内部设置 renderPassEvent
    }

    /// <summary>
    /// 添加渲染通道
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game && pass != null)
        {
            renderer.EnqueuePass(pass);
        }
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (pass is System.IDisposable disposablePass)
            disposablePass.Dispose();
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
            Object.Destroy(material);
        else
            Object.DestroyImmediate(material);
#else
        Object.Destroy(material);
#endif
    }
}

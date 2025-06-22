using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 测试后处理效果的渲染特性
/// </summary>
public class AsciiFeature : ScriptableRendererFeature
{
    [SerializeField] private Material material;
    private AsciiPass asciiPass;

    /// <summary>
    /// 创建渲染通道
    /// </summary>
    public override void Create()
    {
        asciiPass = new AsciiPass(material);
        asciiPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }

    /// <summary>
    /// 添加渲染通道
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        // 只在游戏相机上应用效果
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(asciiPass);
        }
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        asciiPass.Dispose();
        #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Destroy(material);
            }
            else
            {
                DestroyImmediate(material);
            }
        #else
            Destroy(material);
        #endif
    }
}
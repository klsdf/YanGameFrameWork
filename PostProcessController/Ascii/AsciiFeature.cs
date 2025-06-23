using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// ASCII后处理效果的渲染特性
/// </summary>
public class AsciiFeature : YanRenderFeature
{

    [SerializeField] private Shader shader;
    /// <summary>
    /// ASCII渲染通道
    /// </summary>
    private AsciiPass asciiPass;
    [SerializeField] private AsciiSettings settings;
    /// <summary>
    /// 创建渲染通道
    /// </summary>
    public override void Create()
    {
        if (shader == null)
        {
            return;
        }
        material = new Material(shader);
        asciiPass = new AsciiPass(material, settings);
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

[Serializable]
public class AsciiSettings
{
    [Range(5, 100f)] public float strength;

    [SerializeField] public Texture2D asciiTexture;
    [SerializeField] [Range(1, 30)]
    public float asciiSplit = 18;
}
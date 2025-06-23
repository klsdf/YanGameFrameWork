using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEngine.UI;


[System.Serializable]
public class PixelSettings
{    /// <summary>
     /// 像素大小
     /// </summary>
    [Range(1, 50)] public int pixelSize = 8;
}

/// <summary>
/// 像素化后处理效果的渲染特性
/// </summary>
public class PixelRenderFeature : YanRenderFeature
{


    [SerializeField] private Shader pixelShader;

    public PixelSettings pixelSettings;

    /// <summary>
    /// 像素渲染通道
    /// </summary>
    PixelRenderPass pixelPass;



    /// <summary>
    /// 创建渲染通道
    /// </summary>
    public override void Create()
    {
        if (pixelShader != null)
        {
            material = new Material(pixelShader);
        }
        pixelPass = new PixelRenderPass(material, pixelSettings);
        pixelPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }

    /// <summary>
    /// 添加渲染通道
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material != null)
        {
            renderer.EnqueuePass(pixelPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        pixelPass.Dispose();
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



using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 海底扰动后处理渲染功能
/// </summary>
public class UnderSeaDisturbRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private UnderSeaDisturbSettings settings;
    [SerializeField] private Shader shader;
    private Material material;
    private UnderSeaDisturbRenderPass pass;

    public override void Create()
    {
        if (shader == null) return;
        if (material == null)
            material = new Material(shader);
        pass = new UnderSeaDisturbRenderPass(material, settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(pass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (pass != null)
        {
            pass = null;
        }
        if (material != null)
        {
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
}
/// <summary>
/// 海底扰动后处理参数设置
/// </summary>
[System.Serializable]
public class UnderSeaDisturbSettings
{
    /// <summary>
    /// 扰动强度
    /// </summary>
    [Range(0, 0.1f)]
    public float disturbStrength = 0.03f;
    /// <summary>
    /// 扰动频率
    /// </summary>
    [Range(1, 20)]
    public float disturbFrequency = 8f;
    /// <summary>
    /// 扰动速度
    /// </summary>
    [Range(0, 5)]
    public float disturbSpeed = 1f;
}
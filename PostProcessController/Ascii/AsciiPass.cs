using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// ASCII渲染通道，负责执行ASCII后处理效果
/// </summary>
public class AsciiPass : ScriptableRenderPass
{
    /// <summary>
    /// 渲染材质
    /// </summary>
    private Material material;
    private AsciiSettings settings;

    /// <summary>
    /// 纹理描述符
    /// </summary>
    private RenderTextureDescriptor textureDescriptor;
    
    /// <summary>
    /// 纹理句柄
    /// </summary>
    private RTHandle textureHandle;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="material">渲染材质</param>
    public AsciiPass(Material material, AsciiSettings settings)
    {
        this.material = material;
        this.settings = settings;
        textureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
    }

    public override void Configure(CommandBuffer cmd,
        RenderTextureDescriptor cameraTextureDescriptor)
    {
        // Set the blur texture size to be the same as the camera target size.
        textureDescriptor.width = cameraTextureDescriptor.width;
        textureDescriptor.height = cameraTextureDescriptor.height;

        // Check if the descriptor has changed, and reallocate the RTHandle if necessary
        RenderingUtils.ReAllocateIfNeeded(ref textureHandle, textureDescriptor);
    }

    // private void UpdateBlurSettings()
    // {
    //     if (material == null) return;
    //     var volumeComponent =
    //         VolumeManager.instance.stack.GetComponent<AsciiVolumeComponent>();
    //     float intensity = volumeComponent.intensity.overrideState ?
    //         volumeComponent.intensity.value : 1f;

    //     material.SetFloat("_Strength", intensity);
    //     material.SetTexture("_Ascii", volumeComponent.asciiTexture.value);
    // }

    public override void Execute(ScriptableRenderContext context,ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle =
            renderingData.cameraData.renderer.cameraColorTargetHandle;

        material.SetFloat("_Strength", settings.strength);
        material.SetFloat("_AsciiSplit", settings.asciiSplit);
        material.SetTexture("_Ascii", settings.asciiTexture);

        Blit(cmd, cameraTargetHandle, textureHandle);
        Blit(cmd, textureHandle, cameraTargetHandle, material, 0);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
    #if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            Object.Destroy(material);
        }
        else
        {
            Object.DestroyImmediate(material);
        }
    #else
                Object.Destroy(material);
    #endif

        if (textureHandle != null) textureHandle.Release();
    }
}
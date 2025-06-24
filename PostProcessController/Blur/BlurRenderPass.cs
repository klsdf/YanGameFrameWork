using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRenderPass : ScriptableRenderPass
{
    private static readonly int horizontalBlurId =
        Shader.PropertyToID("_HorizontalBlur");
    private static readonly int verticalBlurId =
        Shader.PropertyToID("_VerticalBlur");

    private BlurSettings _settings;
    private Material material;

    private RenderTextureDescriptor blurTextureDescriptor;
    private RTHandle blurTextureHandle;

    public BlurRenderPass(Material material, BlurSettings settings)
    {
        this.material = material;
        this._settings = settings;

        blurTextureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
    }

    public override void Configure(CommandBuffer cmd,
        RenderTextureDescriptor cameraTextureDescriptor)
    {
        // Set the blur texture size to be the same as the camera target size.
        blurTextureDescriptor.width = cameraTextureDescriptor.width;
        blurTextureDescriptor.height = cameraTextureDescriptor.height;

        // Check if the descriptor has changed, and reallocate the RTHandle if necessary
        RenderingUtils.ReAllocateIfNeeded(ref blurTextureHandle, blurTextureDescriptor);
    }

    private void UpdateBlurSettings()
    {
        if (material == null) return;

        // Use the Volume settings or the default settings if no Volume is set.
       
        material.SetFloat(horizontalBlurId, _settings.horizontalBlur);
        material.SetFloat(verticalBlurId, _settings.verticalBlur);
    }

    public override void Execute(ScriptableRenderContext context,
        ref RenderingData renderingData)
    {
        //Get a CommandBuffer from pool.
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle =
            renderingData.cameraData.renderer.cameraColorTargetHandle;

        UpdateBlurSettings();

        // Blit from the camera target to the temporary render texture,
        // using the first shader pass.
        Blit(cmd, cameraTargetHandle, blurTextureHandle, material, 0);
        // Blit from the temporary render texture to the camera target,
        // using the second shader pass.
        Blit(cmd, blurTextureHandle, cameraTargetHandle, material, 1);

        //Execute the command buffer and release it back to the pool.
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

        if (blurTextureHandle != null) blurTextureHandle.Release();
    }
}
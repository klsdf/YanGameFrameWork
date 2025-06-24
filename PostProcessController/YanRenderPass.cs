using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public abstract class YanRenderPass : ScriptableRenderPass
{
    protected RenderTextureDescriptor textureDescriptor;
    protected RTHandle textureHandle;
    protected Material material;
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

        if (textureHandle != null)
            textureHandle.Release();
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
    protected abstract void UpdateSettings();
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle =
            renderingData.cameraData.renderer.cameraColorTargetHandle;

        UpdateSettings();

        Blit(cmd, cameraTargetHandle, textureHandle);
        Blit(cmd, textureHandle, cameraTargetHandle, material, 0);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

}
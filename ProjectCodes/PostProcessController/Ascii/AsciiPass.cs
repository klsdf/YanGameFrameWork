using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AsciiPass : ScriptableRenderPass
{
    private Material material;
    // private TestVolumeComponent volumeComponent;

    private RenderTextureDescriptor textureDescriptor;
    private RTHandle textureHandle;

    public AsciiPass(Material material)
    {
        this.material = material;
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

    private void UpdateBlurSettings()
    {
        if (material == null) return;
        var volumeComponent =
            VolumeManager.instance.stack.GetComponent<AsciiVolumeComponent>();
        float intensity = volumeComponent.intensity.overrideState ?
            volumeComponent.intensity.value : 1f;

        material.SetFloat("_Strength", intensity);
        material.SetTexture("_Ascii", volumeComponent.asciiTexture.value);
    }

    public override void Execute(ScriptableRenderContext context,ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle =
            renderingData.cameraData.renderer.cameraColorTargetHandle;

        UpdateBlurSettings();

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
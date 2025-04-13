using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 像素化后处理
/// </summary>
public class PixelRenderFeature : ScriptableRendererFeature
{
   [Range(1, 50)]
    public int pixelSize = 8;
    public Material pixelationMaterial;
    PixelRenderPass pixelPass;
    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    public override void Create()
    {
        pixelPass = new PixelRenderPass(pixelationMaterial, renderPassEvent, pixelSize);
    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (pixelationMaterial != null)
        {
            renderer.EnqueuePass(pixelPass);
        }
    }

    private class PixelRenderPass : ScriptableRenderPass
    {
        public Material pixelationMaterial;
        private RTHandle m_TempRT;
        public int pixelSize = 8;
        public PixelRenderPass(Material material, RenderPassEvent renderPassEvent, int pixelSize)
        {
            pixelationMaterial = material;
            this.renderPassEvent = renderPassEvent;
            this.pixelSize = pixelSize;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (m_TempRT == null)
            {
                m_TempRT = RTHandles.Alloc(renderingData.cameraData.cameraTargetDescriptor, name: "_TempRT");
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("像素后处理");

            cmd.ClearRenderTarget(true, false, Color.clear);

            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            pixelationMaterial.SetFloat("_PixelSize", pixelSize);

            // 执行像素化处理
            cmd.Blit(source, m_TempRT, pixelationMaterial);
            cmd.Blit(m_TempRT, source);
            

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }


        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (m_TempRT != null)
            {
                RTHandles.Release(m_TempRT);
                m_TempRT = null;
            }
        }
    }
}
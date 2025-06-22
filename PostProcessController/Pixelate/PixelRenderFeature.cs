using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 像素化后处理效果的渲染特性
/// </summary>
public class PixelRenderFeature : YanRenderFeature
{
   /// <summary>
   /// 像素大小
   /// </summary>
   [Range(1, 50)]
    public int pixelSize = 8;

    /// <summary>
    /// 像素渲染通道
    /// </summary>
    PixelRenderPass pixelPass;
    
    /// <summary>
    /// 渲染通道事件
    /// </summary>
    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    
    /// <summary>
    /// 创建渲染通道
    /// </summary>
    public override void Create()
    {
        pixelPass = new PixelRenderPass(material, renderPassEvent, pixelSize);
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

    private class PixelRenderPass : ScriptableRenderPass
    {
        /// <summary>
        /// 像素化材质
        /// </summary>
        public Material pixelationMaterial;
        
        /// <summary>
        /// 临时渲染纹理
        /// </summary>
        private RTHandle m_TempRT;
        
        /// <summary>
        /// 像素大小
        /// </summary>
        public int pixelSize = 8;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="material">渲染材质</param>
        /// <param name="renderPassEvent">渲染通道事件</param>
        /// <param name="pixelSize">像素大小</param>
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
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace YanGameFrameWork.PostProcess
{
    /// <summary>
    /// 模糊效果的渲染通道
    /// </summary>
    public class BlurPass : ScriptableRenderPass
    {
        private Material material;
        private RenderTextureDescriptor textureDescriptor;
        private RTHandle textureHandle;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="material">模糊效果的材质</param>
        public BlurPass(Material material)
        {
            this.material = material;
            textureDescriptor = new RenderTextureDescriptor(Screen.width,
                Screen.height, RenderTextureFormat.Default, 0);
        }

        /// <summary>
        /// 配置渲染通道
        /// </summary>
        public override void Configure(CommandBuffer cmd,
            RenderTextureDescriptor cameraTextureDescriptor)
        {
            textureDescriptor.width = cameraTextureDescriptor.width;
            textureDescriptor.height = cameraTextureDescriptor.height;
            RenderingUtils.ReAllocateIfNeeded(ref textureHandle, textureDescriptor);
        }

        /// <summary>
        /// 执行渲染通道
        /// </summary>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            RTHandle cameraTargetHandle =
                renderingData.cameraData.renderer.cameraColorTargetHandle;

            Blit(cmd, cameraTargetHandle, textureHandle);
            Blit(cmd, textureHandle, cameraTargetHandle, material, 0);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            if (textureHandle != null) textureHandle.Release();
        }
    }
} 
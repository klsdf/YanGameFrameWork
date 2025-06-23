using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 海底扰动后处理渲染通道
/// </summary>
public class UnderSeaDisturbRenderPass : ScriptableRenderPass
{
    private Material _material;
    private UnderSeaDisturbSettings _settings;
    private RTHandle _tempRT;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UnderSeaDisturbRenderPass(Material material, UnderSeaDisturbSettings settings)
    {
        _material = material;
        _settings = settings;
        renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (_tempRT == null)
        {
            _tempRT = RTHandles.Alloc(renderingData.cameraData.cameraTargetDescriptor, name: "_UnderSeaTempRT");
        }
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("海底扰动后处理");

        // 设置参数
        _material.SetFloat("_DisturbStrength", _settings.disturbStrength);
        _material.SetFloat("_DisturbFrequency", _settings.disturbFrequency);
        _material.SetFloat("_DisturbSpeed", _settings.disturbSpeed);

        var source = renderingData.cameraData.renderer.cameraColorTargetHandle;

        cmd.Blit(source, _tempRT, _material);
        cmd.Blit(_tempRT, source);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (_tempRT != null)
        {
            RTHandles.Release(_tempRT);
            _tempRT = null;
        }
    }
}

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MyDitheringRenderPass : ScriptableRenderPass
{
    // private static readonly int intensityId =
    //     Shader.PropertyToID("_Intensity");

    private MyDitheringSettings _settings;
    private Material material;

    private RenderTextureDescriptor textureDescriptor;
    private RTHandle textureHandle;

    public MyDitheringRenderPass(Material material, MyDitheringSettings settings)
    {
        this.material = material;
        this._settings = settings;

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

    /// <summary>
    /// 更新材质参数，将MyDitheringSettings中的参数传递给Shader
    /// </summary>
    private void UpdateMyDitheringSettings()
    {
        if (material == null || _settings == null) return;

        // 设置抖动图案纹理
        material.SetTexture("_PatternTex", _settings.patternTexture);

        // 设置抖动图案缩放
        material.SetFloat("_PatternScale", _settings.patternScale);

        // 设置抖动颜色数量
        material.SetFloat("_DitherColorCount", _settings.ditherColorCount);

        // 设置抖动颜色数组（最多8种）
        for (int i = 0; i < 8; i++)
        {
            string colorProp = $"_DitherColors{i}";
            if (_settings.ditherColors != null && i < _settings.ditherColors.Length)
            {
                material.SetColor(colorProp, _settings.ditherColors[i]);
            }
            else
            {
                // 超出部分用黑色填充
                material.SetColor(colorProp, Color.black);
            }
        }

    }

    public override void Execute(ScriptableRenderContext context,ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();

        RTHandle cameraTargetHandle =
            renderingData.cameraData.renderer.cameraColorTargetHandle;

        UpdateMyDitheringSettings();

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
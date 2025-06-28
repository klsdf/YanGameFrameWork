using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 抖动后处理渲染通道，对应Dithering2.shader
/// </summary>
public class MyDitheringRenderPass2 : YanRenderPass
{
    /// <summary>
    /// 抖动后处理设置
    /// </summary>
    private MyDitheringSettings2 _settings;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="material">抖动效果的材质</param>
    /// <param name="settings">抖动效果的设置参数</param>
    public MyDitheringRenderPass2(Material material, MyDitheringSettings2 settings)
    {
        this.material = material;
        this._settings = settings;

        // 初始化渲染纹理描述符
        textureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
    }

    /// <summary>
    /// 更新材质参数，将MyDitheringSettings2中的参数传递给Shader
    /// 对应Dithering2.shader中的Properties
    /// </summary>
    protected override void UpdateSettings()
    {
        // 安全检查
        if (material == null)
        {
            Debug.LogWarning("MyDitheringRenderPass2: material is null");
            return;
        }
        
        if (_settings == null)
        {
            Debug.LogWarning("MyDitheringRenderPass2: settings is null");
            return;
        }

        // 设置调色板相关参数 (对应 _PaletteColorCount, _PaletteHeight, _PaletteTex)
        material.SetFloat("_PaletteColorCount", _settings.paletteColorCount);
        material.SetFloat("_PaletteHeight", _settings.paletteHeight);
        material.SetTexture("_PaletteTex", _settings.paletteTex);

        // 设置抖动图案相关参数 (对应 _PatternSize, _PatternTex, _PatternScale)
        material.SetFloat("_PatternSize", _settings.patternSize);
        material.SetTexture("_PatternTex", _settings.patternTex);
        material.SetFloat("_PatternScale", _settings.patternScale);
    }
}
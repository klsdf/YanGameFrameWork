using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MyDitheringRenderPass2 : YanRenderPass
{
    // private static readonly int intensityId =
    //     Shader.PropertyToID("_Intensity");

    private MyDitheringSettings2 _settings;

    public MyDitheringRenderPass2(Material material, MyDitheringSettings2 settings)
    {
        this.material = material;
        this._settings = settings;

        textureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
    }



    /// <summary>
    /// 更新材质参数，将MyDitheringSettings中的参数传递给Shader
    /// </summary>
    protected override void UpdateSettings()
    {
        if (material == null || _settings == null) return;

        // 设置调色板相关
        material.SetTexture("_PaletteTex", _settings.paletteTex);
        material.SetFloat("_PaletteColorCount", _settings.paletteColorCount);
        material.SetFloat("_PaletteHeight", _settings.paletteHeight);

        // 设置抖动图案相关
        material.SetTexture("_PatternTex", _settings.patternTex);
        material.SetFloat("_PatternScale", _settings.patternScale);
        material.SetFloat("_PatternSize", _settings.patternSize);
    }


}
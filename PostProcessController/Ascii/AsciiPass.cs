using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// ASCII渲染通道，负责执行ASCII后处理效果
/// </summary>
public class AsciiPass : YanRenderPass
{


    private AsciiSettings settings;



    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="material">渲染材质</param>
    public AsciiPass(Material material, AsciiSettings settings)
    {
        this.material = material;
        this.settings = settings;
        textureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
    }

    protected override void UpdateSettings()
    {

        material.SetFloat("_Strength", settings.strength);
        material.SetFloat("_AsciiSplit", settings.asciiSplit);
        material.SetTexture("_Ascii", settings.asciiTexture);
    }




}
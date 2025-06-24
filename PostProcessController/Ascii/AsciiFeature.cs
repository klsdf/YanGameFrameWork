using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// ASCII后处理效果的渲染特性
/// </summary>
public class AsciiFeature : YanRenderFeature<AsciiPass, AsciiSettings>
{
    /// <summary>
    /// 创建ASCII渲染通道
    /// </summary>
    protected override AsciiPass CreatePass(Material material, AsciiSettings settings)
    {
        var pass = new AsciiPass(material, settings);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        return pass;
    }

  
}

[Serializable]
public class AsciiSettings
{
    [Range(5, 100f)] public float strength;

    [SerializeField] public Texture2D asciiTexture;
    [SerializeField] [Range(1, 30)]
    public float asciiSplit = 18;
}
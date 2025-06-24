using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorFliterRenderPass : YanRenderPass
{
    // private static readonly int intensityId =
    //     Shader.PropertyToID("_Intensity");

    private RedTintSettings _settings;
    public ColorFliterRenderPass(Material material, RedTintSettings settings)
    {
        this.material = material;
        this._settings = settings;

        textureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
    }


    protected override void UpdateSettings()
    {
        if (material == null) return;
        material.SetFloat("_Intensity", _settings.intensity);
        material.SetColor("_Color", _settings.color);
    }
}
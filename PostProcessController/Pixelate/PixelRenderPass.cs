using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelRenderPass : YanRenderPass
{
    private PixelSettings _settings;


    public PixelRenderPass(Material material, PixelSettings settings)
    {
        this.material = material;
        this._settings = settings;

        textureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
    }

 

    protected override void UpdateSettings()
    {
        if (material == null) return;
        material.SetFloat("_PixelSize", _settings.pixelSize);
    }

}
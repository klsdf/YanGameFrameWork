using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RedBorderRenderPass : YanRenderPass
{
    private RedBorderSettings settings;
    
    public RedBorderRenderPass(Material material, RedBorderSettings settings)
    {
        this.material = material;
        this.settings = settings;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        textureDescriptor = new RenderTextureDescriptor(Screen.width,
            Screen.height, RenderTextureFormat.Default, 0);
        
    }
    
    protected override void UpdateSettings()
    {
        if (material != null && settings != null)
        {
            material.SetColor("_BorderColor", settings.borderColor);
            material.SetFloat("_BorderWidth", settings.borderWidth);
            material.SetFloat("_BorderIntensity", settings.borderIntensity);
        }
    }
}

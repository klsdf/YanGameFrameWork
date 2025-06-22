using UnityEngine.Rendering.Universal;
using UnityEngine;

public abstract class YanRenderFeature : ScriptableRendererFeature
{
    public Material material;

    public void SetMaterial(Material material)
    {
        this.material = material;
    }
}

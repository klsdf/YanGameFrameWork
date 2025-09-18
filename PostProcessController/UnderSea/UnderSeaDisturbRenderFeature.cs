using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UnderSeaDisturbRenderFeature : YanRenderFeature<UnderSeaDisturbRenderPass, UnderSeaDisturbSettings>
{
    [SerializeField] private RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

    protected override UnderSeaDisturbRenderPass CreatePass(Material material, UnderSeaDisturbSettings settings)
    {
        var pass = new UnderSeaDisturbRenderPass(material, settings);
        pass.renderPassEvent = _renderPassEvent;
        return pass;
    }
}



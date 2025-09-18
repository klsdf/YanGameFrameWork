using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UnderSeaDisturbRenderPass : YanRenderPass
{
    private readonly Material _material;
    private readonly UnderSeaDisturbSettings _settings;

    private static readonly int BlueTintColorId = Shader.PropertyToID("_BlueTintColor");
    private static readonly int RippleAmountId = Shader.PropertyToID("_RippleAmount");
    private static readonly int RippleFrequencyId = Shader.PropertyToID("_RippleFrequency");
    private static readonly int RippleSpeedId = Shader.PropertyToID("_RippleSpeed");
    private static readonly int NoiseScaleId = Shader.PropertyToID("_NoiseScale");
    private static readonly int NoiseThresholdId = Shader.PropertyToID("_NoiseThreshold");
    private static readonly int BubbleIntensityId = Shader.PropertyToID("_BubbleIntensity");

    public UnderSeaDisturbRenderPass(Material material, UnderSeaDisturbSettings settings)
    {
        _material = material;
        this.material = material;
        _settings = settings;
        renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        textureDescriptor = new RenderTextureDescriptor(1, 1);
    }

    protected override void UpdateSettings()
    {
        if (_settings == null) return;

        _material.SetColor(BlueTintColorId, _settings.blueTintColor);

        float ripple = _settings.baseRippleStrength;
        _material.SetFloat(RippleAmountId, ripple);
        _material.SetFloat(RippleFrequencyId, _settings.rippleFrequency);
        _material.SetFloat(RippleSpeedId, _settings.rippleSpeed);
        _material.SetFloat(NoiseScaleId, _settings.noiseScale);
        _material.SetFloat(NoiseThresholdId, _settings.noiseThreshold);
        _material.SetFloat(BubbleIntensityId, _settings.bubbleIntensity);
    }
}



using UnityEngine;

[System.Serializable]
public class UnderSeaDisturbSettings : YanRenderFeatureSetting
{
    public Color blueTintColor = new Color(0.4f, 0.6f, 1f, 0.25f);

    [Header("Ripple Base")]
    [Range(0f, 0.1f)] public float baseRippleStrength = 0.02f;
    [Range(0.5f, 20f)] public float rippleFrequency = 10f;
    [Range(0.1f, 10f)] public float rippleSpeed = 2.0f;

    [Header("Local Bubble Noise Distortion")]
    [Range(0.5f, 10f)] public float noiseScale = 3.0f;
    [Range(0f, 1f)] public float noiseThreshold = 0.7f;
    [Range(0f, 0.1f)] public float bubbleIntensity = 0.04f;
}



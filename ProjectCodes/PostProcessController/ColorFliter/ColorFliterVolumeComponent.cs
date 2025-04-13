using System;
using UnityEngine.Rendering;
using UnityEngine;

[Serializable]
public class ColorFliterVolumeComponent : VolumeComponent
{
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0.05f, 0, 0.5f);
    public ColorParameter color = new ColorParameter( Color.red);
}
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MyDitheringRenderFeature : YanRenderFeature<MyDitheringRenderPass, MyDitheringSettings>
{
  protected override MyDitheringRenderPass CreatePass(Material material, MyDitheringSettings settings)
    {
        var pass = new MyDitheringRenderPass(material, settings);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
        return pass;
    }
}


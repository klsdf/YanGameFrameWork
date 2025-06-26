using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MyDitheringRenderFeature2 : YanRenderFeature<MyDitheringRenderPass2, MyDitheringSettings2>
{
  protected override MyDitheringRenderPass2 CreatePass(Material material, MyDitheringSettings2 settings)
  {
    var pass = new MyDitheringRenderPass2(material, settings);
    pass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    return pass;
  }
}


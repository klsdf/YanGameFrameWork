using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MyDitheringRenderFeature : YanRenderFeature
{
    [SerializeField] private MyDitheringSettings settings;
    private MyDitheringRenderPass myDitheringRenderPass;    

    public override void Create()
    {
        if (shader == null)
        {
            return;
        }
        material = new Material(shader);
        myDitheringRenderPass = new MyDitheringRenderPass(material, settings);
        
        myDitheringRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(myDitheringRenderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        myDitheringRenderPass.Dispose();
        #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Destroy(material);
            }
            else
            {
                DestroyImmediate(material);
            }
        #else
                Destroy(material);
        #endif
    }
}


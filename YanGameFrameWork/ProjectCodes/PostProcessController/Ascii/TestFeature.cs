using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class TestFeature : ScriptableRendererFeature
{
    [SerializeField] private Material material;
    private TestPass testPass;

    public override void Create()
    {
        testPass = new TestPass(material);
        
        testPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(testPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        testPass.Dispose();
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
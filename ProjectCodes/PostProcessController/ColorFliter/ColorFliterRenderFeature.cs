using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ColorFliterRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private RedTintSettings settings;
    [SerializeField] private Shader shader;
    private Material material;
    private ColorFliterRenderPass colorFliterRenderPass;

    public override void Create()
    {
        if (shader == null)
        {
            return;
        }
        material = new Material(shader);
        colorFliterRenderPass = new ColorFliterRenderPass(material, settings);
        
        colorFliterRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(colorFliterRenderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        colorFliterRenderPass.Dispose();
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

[Serializable]
public class RedTintSettings
{
    [Range(0, 1f)] public float intensity;
    public Color color;
}
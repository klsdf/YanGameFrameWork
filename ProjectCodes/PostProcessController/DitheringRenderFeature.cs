using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DitheringRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    [Header("调色板")]
    private Palette _palette; // 用于抖动效果的颜色调色板
    public Palette Palette { get { return _palette; } set { _palette = value; } }


    [SerializeField]
    [Header("抖动效果的模式/图案")]
    private Pattern _pattern;// 抖动效果的模式/图案
    public Pattern Pattern { get { return _pattern; } set { _pattern = value; } }

    [SerializeField]
    [Header("自定义的抖动纹理")]
    private Texture2D _patternTexture;// 自定义的抖动纹理
    public Texture2D PatternTexture { get { return _patternTexture; } set { _patternTexture = value; } }

    private class DitheringPass : ScriptableRenderPass
    {
        public Material grayscaleMaterial;
        public Palette palette;
        public Pattern pattern;
        public Texture2D patternTexture;

        private RTHandle m_TempRT;
        public DitheringPass(Material material, RenderPassEvent renderPassEvent, Palette palette, Pattern pattern, Texture2D patternTexture)
        {
            grayscaleMaterial = material;
            this.renderPassEvent = renderPassEvent;
            this.palette = palette;
            this.pattern = pattern;
            this.patternTexture = patternTexture;
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (m_TempRT == null)
            {
                m_TempRT = RTHandles.Alloc(renderingData.cameraData.cameraTargetDescriptor, name: "_TempRT");
            }
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("自定义后处理");

            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            /*

            测试部分
            */

            if (palette == null || (pattern == null && patternTexture == null))
            {
                return;
            }

            if (!palette.HasTexture || (patternTexture == null && !pattern.HasTexture))
            {
                return;
            }

            Texture2D patTex = (pattern == null ? patternTexture : pattern.Texture);

            grayscaleMaterial.SetFloat("_PaletteColorCount", palette.MixedColorCount);
            grayscaleMaterial.SetFloat("_PaletteHeight", palette.Texture.height);
            grayscaleMaterial.SetTexture("_PaletteTex", palette.Texture);
            grayscaleMaterial.SetFloat("_PatternSize", patTex.width);
            grayscaleMaterial.SetTexture("_PatternTex", patTex);
            /*

            测试结束
            */
            cmd.Blit(source, m_TempRT, grayscaleMaterial);
            cmd.Blit(m_TempRT, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (m_TempRT != null)
            {
                RTHandles.Release(m_TempRT);
                m_TempRT = null;
            }
        }
    }

    public Material ditheringMaterial;
    DitheringPass ditheringPass;
    private RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;


    public override void Create()
    {
        ditheringPass = new DitheringPass(ditheringMaterial, renderPassEvent, Palette, Pattern, PatternTexture);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (ditheringMaterial != null)
        {
            renderer.EnqueuePass(ditheringPass);
        }
    }
}
using UnityEngine;
using YanGameFrameWork.Singleton;
using UnityEngine.Rendering.Universal;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;

namespace YanGameFrameWork.PostProcess
{
    public enum PostProcessType
    {
        Ascii,
        ColorFliter,
    }

    /// <summary>
    /// 后处理管理器
    /// 负责管理游戏中的各种后处理效果，包括泛光、景深、色彩调整等
    /// </summary>
    public class PostProcessController : Singleton<PostProcessController>
    {
        /// <summary>
        /// 当前启用的后处理效果
        /// </summary>
        private PostProcessType currentEffect = PostProcessType.Ascii;

        /// <summary>
        /// URP渲染器数据
        /// </summary>
        public Renderer2DData rendererData;


        /// <summary>
        /// 切换后处理效果
        /// </summary>
        /// <param name="type">要切换到的后处理类型</param>
        [Button("切换后处理效果")]
        public void ChangePostProcess(PostProcessType type)
        {
            if (rendererData == null)
            {
                Debug.LogError("Renderer Data未初始化，请先调用Initialize()");
               
                return;
            }

            // 禁用当前效果
            DisableCurrentEffect();

            // 启用新效果
            EnableEffect(type);

            currentEffect = type;
        }

        /// <summary>
        /// 禁用当前效果
        /// </summary>
        private void DisableCurrentEffect()
        {
            switch (currentEffect)
            {
                case PostProcessType.Ascii:
                    DisableRendererFeature<AsciiFeature>();
                    break;
                case PostProcessType.ColorFliter:
                    DisableRendererFeature<ColorFliterRenderFeature>();
                    break;
            }
        }

        /// <summary>
        /// 启用指定效果
        /// </summary>
        /// <param name="type">要启用的效果类型</param>
        private void EnableEffect(PostProcessType type)
        {
            switch (type)
            {
                case PostProcessType.Ascii:
                    EnableRendererFeature<AsciiFeature>();
                    break;
                case PostProcessType.ColorFliter:
                    EnableRendererFeature<ColorFliterRenderFeature>();
                    break;
            }
        }

        /// <summary>
        /// 启用指定类型的Renderer Feature
        /// </summary>
        /// <typeparam name="T">Renderer Feature类型</typeparam>
        private void EnableRendererFeature<T>() where T : ScriptableRendererFeature
        {
            var feature = GetOrCreateRendererFeature<T>();
            if (feature != null)
            {
                feature.SetActive(true);
            }
        }

        /// <summary>
        /// 禁用指定类型的Renderer Feature
        /// </summary>
        /// <typeparam name="T">Renderer Feature类型</typeparam>
        private void DisableRendererFeature<T>() where T : ScriptableRendererFeature
        {
            var feature = GetOrCreateRendererFeature<T>();
            if (feature != null)
            {
                feature.SetActive(false);
            }
        }

        /// <summary>
        /// 获取或创建指定类型的Renderer Feature
        /// </summary>
        /// <typeparam name="T">Renderer Feature类型</typeparam>
        /// <returns>Renderer Feature实例</returns>
        private T GetOrCreateRendererFeature<T>() where T : ScriptableRendererFeature
        {
            // 查找已存在的Feature
            foreach (var feature in rendererData.rendererFeatures)
            {
                if (feature is T targetFeature)
                {
                    return targetFeature;
                }
            }

            // 如果不存在，创建新的Feature
            var newFeature = ScriptableObject.CreateInstance<T>();
            rendererData.rendererFeatures.Add(newFeature);
            return newFeature;
        }
    }
}
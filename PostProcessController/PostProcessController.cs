using UnityEngine;
using YanGameFrameWork.Singleton;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System;


namespace YanGameFrameWork.PostProcess
{
    public enum PostProcessType
    {
        Ascii,
        ColorFliter,
        Blur,
        Pixelate,
    }




    [Serializable]
    public class PostProcessData
    {
        public PostProcessType postProcessType;
        public Material material;

    }

    /// <summary>
    /// 后处理管理器
    /// 负责管理游戏中的各种后处理效果，包括泛光、景深、色彩调整等
    /// </summary>
    public class PostProcessController : Singleton<PostProcessController>
    {


        [SerializeField] private List<PostProcessData> postProcessDataList;
        /// <summary>
        /// 当前启用的后处理效果
        /// </summary>
        private PostProcessType currentEffect = PostProcessType.Ascii;

        /// <summary>
        /// URP渲染器数据
        /// </summary>
        public Renderer2DData rendererData;


        // /// <summary>
        // /// 切换后处理效果
        // /// </summary>
        // /// <param name="type">要切换到的后处理类型</param>
        // [Button("切换后处理效果")]
        // public void ChangePostProcess(PostProcessType type)
        // {
        //     if (rendererData == null)
        //     {
        //         Debug.LogError("Renderer Data未初始化，请先调用Initialize()");

        //         return;
        //     }

        //     // 禁用当前效果
        //     DisableCurrentEffect();

        //     // 启用新效果
        //     EnableEffect(type);

        //     currentEffect = type;
        // }

        // /// <summary>
        // /// 禁用当前效果
        // /// </summary>
        // private void DisableCurrentEffect()
        // {
        //     switch (currentEffect)
        //     {
        //         case PostProcessType.Ascii:
        //             DisableRendererFeature<AsciiFeature>();
        //             break;
        //         case PostProcessType.ColorFliter:
        //             DisableRendererFeature<ColorFliterRenderFeature>();
        //             break;
        //         case PostProcessType.Blur:
        //             DisableRendererFeature<BlurRendererFeature>();
        //             break;
        //         case PostProcessType.Pixelate:
        //             DisableRendererFeature<PixelRenderFeature>();
        //             break;
        //     }
        // }

        // /// <summary>
        // /// 启用指定效果
        // /// </summary>
        // /// <param name="type">要启用的效果类型</param>
        // private void EnableEffect(PostProcessType type)
        // {
        //     switch (type)
        //     {
        //         case PostProcessType.Ascii:
        //             EnableRendererFeature<AsciiFeature>();
        //             break;
        //         case PostProcessType.ColorFliter:
        //             EnableRendererFeature<ColorFliterRenderFeature>();
        //             break;
        //         case PostProcessType.Blur:
        //             EnableRendererFeature<BlurRendererFeature>();
        //             break;
        //         case PostProcessType.Pixelate:
        //             EnableRendererFeature<PixelRenderFeature>();
        //             break;
        //     }
        // }

        // /// <summary>
        // /// 启用指定类型的Renderer Feature
        // /// </summary>
        // /// <typeparam name="T">Renderer Feature类型</typeparam>
        // private void EnableRendererFeature<T>() where T : YanRenderFeature
        // {
        //     var feature = GetOrCreateRendererFeature<T>();
        //     if (feature != null)
        //     {
        //         feature.SetActive(true);
        //     }
        // }

        // /// <summary>
        // /// 禁用指定类型的Renderer Feature
        // /// </summary>
        // /// <typeparam name="T">Renderer Feature类型</typeparam>
        // private void DisableRendererFeature<T>() where T : YanRenderFeature
        // {
        //     var feature = GetOrCreateRendererFeature<T>();
        //     if (feature != null)
        //     {
        //         feature.SetActive(false);
        //     }
        // }

        // /// <summary>
        // /// 获取或创建指定类型的Renderer Feature
        // /// </summary>
        // /// <typeparam name="T">Renderer Feature类型</typeparam>
        // /// <returns>Renderer Feature实例</returns>
        // private T GetOrCreateRendererFeature<T>() where T : YanRenderFeature
        // {
        //     // 查找已存在的Feature
        //     foreach (var feature in rendererData.rendererFeatures)
        //     {
        //         if (feature is T targetFeature)
        //         {
        //             return targetFeature;
        //         }
        //     }

        //     // 如果不存在，创建新的Feature
        //     var newFeature = ScriptableObject.CreateInstance<T>();

        //     switch (newFeature)
        //     {
        //         case AsciiFeature asciiFeature:
        //             asciiFeature.SetMaterial(postProcessDataList.Find(data => data.postProcessType == PostProcessType.Ascii).material);
        //             break;
        //         case ColorFliterRenderFeature colorFliterRenderFeature:
        //             colorFliterRenderFeature.SetMaterial(postProcessDataList.Find(data => data.postProcessType == PostProcessType.ColorFliter).material);
        //             break;
        //         case BlurRendererFeature blurRendererFeature:
        //             blurRendererFeature.SetMaterial(postProcessDataList.Find(data => data.postProcessType == PostProcessType.Blur).material);
        //             break;
        //         case PixelRenderFeature pixelRenderFeature:
        //             pixelRenderFeature.SetMaterial(postProcessDataList.Find(data => data.postProcessType == PostProcessType.Pixelate).material);
        //             break;
        //      }
        //     rendererData.rendererFeatures.Add(newFeature);

        //     return newFeature;
        // }

		/// <summary>
		/// 启用或禁用 UnderSeaDisturbRenderFeature
		/// </summary>
		/// <param name="enabled">true 启用，false 禁用</param>
		public void SetUnderSeaDisturbEnabled(bool enabled)
		{
			if (rendererData == null)
			{
				Debug.LogError("Renderer Data 未设置，无法切换 UnderSeaDisturbRenderFeature");
				return;
			}

			bool found = false;
			foreach (var feature in rendererData.rendererFeatures)
			{
				if (feature is UnderSeaDisturbRenderFeature target)
				{
					target.SetActive(enabled);
					found = true;
					break;
				}
			}

			if (!found)
			{
				Debug.LogWarning("未在 RendererData 中找到 UnderSeaDisturbRenderFeature");
			}
		}
    }
}

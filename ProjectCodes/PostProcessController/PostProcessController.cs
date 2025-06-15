using UnityEngine;
using YanGameFrameWork.Singleton;


namespace YanGameFrameWork.PostProcess
{

    public enum PostProcessType
    {
        Blur,
        ColorFilter,
        Test
    }

    /// <summary>
    /// 后处理管理器
    /// 负责管理游戏中的各种后处理效果，包括泛光、景深、色彩调整等
    /// </summary>
    public class PostProcessController : Singleton<PostProcessController>
    {

        public void ChangePostProcess(PostProcessType type)
        {
            switch (type)
            {
                case PostProcessType.Blur:
                    break;
                case PostProcessType.ColorFilter:
                    break;
                case PostProcessType.Test:
                    break;
            }
        }
    }
}
/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-28
 * Description: 游戏框架的静态类，用于简化代码，使其可以快速访问常见的控制类
 *
 ****************************************************************************/
using YanGameFrameWork.SceneControlSystem;
using YanGameFrameWork.UISystem;
using YanGameFrameWork.SaveSystem;
using YanGameFrameWork.LocalizationSystem;
using YanGameFrameWork.AudioSystem;
using YanGameFrameWork.AchievementSystem;
using YanGameFrameWork.EventSystem;
using YanGameFrameWork.ModelControlSystem;
using YanGameFrameWork.ResourceControlSystem;
/// <summary>
/// 游戏框架的静态类，用于简化代码，使其可以快速访问常见的控制类，可以直接使用
/// </summary>
public static class YanGF
{

    /// <summary>
    /// 场景控制器，全称是SceneController
    /// </summary>
    public static SceneController Scene
    {
        get
        {

            return SceneController.Instance;
        }
    }


    public static AudioController Audio
    {
        get
        {
            return AudioController.Instance;
        }
    }


    /// <summary>
    /// UI控制器，全称是UIController
    /// </summary>
    public static UIController UI
    {
        get
        {
            return UIController.Instance;
        }
    }


    /// <summary>
    /// 保存控制器，全称是SaveController
    /// </summary>
    public static SaveController Save
    {
        get
        {
            return SaveController.Instance;
        }
    }


    /// <summary>
    /// 本地化控制器，全称是LocalizationController
    /// </summary>
    public static LocalizationController Localization
    {
        get
        {
            return LocalizationController.Instance;
        }
    }

    /// <summary>
    /// 成就系统，全称是AchievementSystem
    /// </summary>
    public static AchievementSystem Achievement
    {
        get
        {
            return AchievementSystem.Instance;
        }
    }



    /// <summary>
    /// 调试控制器，全称是DebugController
    /// </summary>
    public static DebugController Debug
    {
        get
        {
            return DebugController.Instance;
        }
    }


    /// <summary>
    /// 事件系统，全称是EventSystemController
    /// </summary>
    public static EventSystemController Event
    {
        get
        {
            return EventSystemController.Instance;
        }
    }


    /// <summary>
    /// 数据控制器，全称是ModelController
    /// </summary>
    public static ModelController Model
    {
        get
        {
            return ModelController.Instance;
        }
    }


    /// <summary>
    /// 资源控制器，全称是ResourcesController
    /// </summary>
    public static ResourcesController Resources
    {
        get
        {
            return ResourcesController.Instance;
        }
    }

}


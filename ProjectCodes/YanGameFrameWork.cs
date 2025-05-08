/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-28
 * Description: 游戏框架的静态类，用于简化代码，使其可以快速访问常见的控制类

 * 修改记录
 * 2025-04-13 闫辰祥 发现引入项目时，可能导致项目中存在多个相同的控制器，导致冲突，所以显式用命名空间来访问控制器
 ****************************************************************************/
using YanGameFrameWork.SceneControlSystem;
using YanGameFrameWork.SaveSystem;
using YanGameFrameWork.LocalizationSystem;
using YanGameFrameWork.AudioSystem;
using YanGameFrameWork.AchievementSystem;
using YanGameFrameWork.EventSystem;
using YanGameFrameWork.ModelControlSystem;
using YanGameFrameWork.ResourceControlSystem;
using YanGameFrameWork.CursorController;
using YanGameFrameWork.CameraController;
/// <summary>
/// 游戏框架的静态类，用于简化代码，使其可以快速访问常见的控制类，可以直接使用
/// </summary>
public static class YanGF
{

    /// <summary>
    /// 场景控制器，全称是SceneController
    /// </summary>
    public static YanGameFrameWork.SceneControlSystem.SceneController Scene
    {
        get
        {

            return YanGameFrameWork.SceneControlSystem.SceneController.Instance;
        }
    }


    public static YanGameFrameWork.AudioSystem.AudioController Audio
    {
        get
        {
            return YanGameFrameWork.AudioSystem.AudioController.Instance;
        }
    }


    /// <summary>
    /// UI控制器，全称是UIController
    /// </summary>
    public static YanGameFrameWork.UISystem.UIController UI
    {
        get
        {
            return YanGameFrameWork.UISystem.UIController.Instance;
        }
    }


    /// <summary>
    /// 保存控制器，全称是SaveController
    /// </summary>
    public static YanGameFrameWork.SaveSystem.SaveController Save
    {
        get
        {
            return YanGameFrameWork.SaveSystem.SaveController.Instance;
        }
    }


    /// <summary>
    /// 本地化控制器，全称是LocalizationController
    /// </summary>
    public static YanGameFrameWork.LocalizationSystem.LocalizationController Localization
    {
        get
        {
            return YanGameFrameWork.LocalizationSystem.LocalizationController.Instance;
        }
    }

    /// <summary>
    /// 成就系统，全称是AchievementSystem
    /// </summary>
    public static YanGameFrameWork.AchievementSystem.AchievementSystem Achievement
    {
        get
        {
            return YanGameFrameWork.AchievementSystem.AchievementSystem.Instance;
        }
    }



    /// <summary>
    /// 调试控制器，全称是DebugController
    /// </summary>
    public static YanGameFrameWork.DebugSystem.DebugController Debug
    {
        get
        {
            return YanGameFrameWork.DebugSystem.DebugController.Instance;
        }
    }


    /// <summary>
    /// 事件系统，全称是EventSystemController
    /// </summary>
    public static YanGameFrameWork.EventSystem.EventSystemController Event
    {
        get
        {
            return YanGameFrameWork.EventSystem.EventSystemController.Instance;
        }
    }


    /// <summary>
    /// 数据控制器，全称是ModelController
    /// </summary>
    public static YanGameFrameWork.ModelControlSystem.ModelController Model
    {
        get
        {
            return YanGameFrameWork.ModelControlSystem.ModelController.Instance;
        }
    }


    /// <summary>
    /// 资源控制器，全称是ResourcesController
    /// </summary>
    public static YanGameFrameWork.ResourceControlSystem.ResourcesController Resources
    {
        get
        {
            return YanGameFrameWork.ResourceControlSystem.ResourcesController.Instance;
        }
    }

    /// <summary>
    /// 光标控制器，全称是CursorManager
    /// </summary>
    public static YanGameFrameWork.CursorController.CursorManager Cursor
    {
        get
        {
            return YanGameFrameWork.CursorController.CursorManager.Instance;
        }
    }


    /// <summary>
    /// 摄像机控制器，全称是CameraController
    /// </summary>
    public static YanGameFrameWork.CameraController.CameraController Camera
    {
        get
        {
            return YanGameFrameWork.CameraController.CameraController.Instance;
        }
    }

    /// <summary>
    /// Tween控制器，全称是TweenController
    /// </summary>
    public static YanGameFrameWork.TweenSystem.TweenController Tween
    {
        get
        {
            return YanGameFrameWork.TweenSystem.TweenController.Instance;
        }
    }


    /// <summary>
    /// 时间控制器，全称是TimeController
    /// </summary>
    public static YanGameFrameWork.TimeControlSystem.TimeController Timer
    {
        get
        {
            return YanGameFrameWork.TimeControlSystem.TimeController.Instance;
        }
    }


    /// <summary>
    /// 对话控制器，全称是DialogController
    /// </summary>
    public static YanGameFrameWork.DialogSystem.DialogController Dialog
    {
        get
        {
            return YanGameFrameWork.DialogSystem.DialogController.Instance;
        }
    }



    /// <summary>
    /// 新手引导控制器，全称是MaskedUI
    /// </summary>
    public static YanGameFrameWork.TutoriaSystem.TutoriaController Tutoria
    {
        get
        {
            return YanGameFrameWork.TutoriaSystem.TutoriaController.Instance;
        }
    }

}


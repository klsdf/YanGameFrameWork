/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-16
 * Description: 成就系统示例，以及单元测试用的类
 ****************************************************************************/

using UnityEngine;
using YanGameFrameWork.AchievementSystem;
using YanGameFrameWork.Editor;

public class ExampleAchievementSystem : MonoBehaviour
{

    [Button("测试注册成就")]
    public void TestRegisterAchievement()
    {
        YanGF.Achievement.RegisterAchievement(new EventAchievement("梦的开始", "第一次进入游戏", () =>
        {
            print("梦的开始");
        }));
        YanGF.Achievement.RegisterAchievement(new ProgressAchievement("靓仔，要来点拼好饭吗？", "赚到100块钱", 100, () =>
        {
            print("靓仔，要来点拼好饭吗？");
        }));
    }

    [Button("测试解锁成就")]
    public void TestUnlockAchievement()
    {
        YanGF.Achievement.UpdateOrUnlockAchievement("梦的开始");
        YanGF.Achievement.UpdateOrUnlockAchievement("靓仔，要来点拼好饭吗？", 10);
    }


}

using System;

namespace YanGameFrameWork.AchievementSystem
{
    /// <summary>
    /// 事件型成就，也就是开始游戏，战胜BOSS等事件触发的成就
    /// </summary>
    public class EventAchievement : AchievementBase
    {
        public EventAchievement(string title, string description, Action onUnlock = null)
            : base(title, description, onUnlock)
        {

        }

        /// <summary>
        /// 更新或解锁成就
        /// </summary>
        /// <returns>本次是否解锁</returns>
        public override bool UpdateOrUnlock()
        {
            if (isUnlock)
            {
                return false;
            }

            isUnlock = true;
            OnUnlock?.Invoke();
            return true;
        }


        /// <summary>
        /// 更新或解锁成就
        /// </summary>
        /// <param name="value">增加的进度值</param>
        /// <returns>本次是否解锁</returns>
        public override bool UpdateOrUnlock(int value)
        {
            YanGF.Debug.LogWarning(nameof(EventAchievement), $"正在试图给一个事件型成就“{title}”增加进度值，请检查配置是否正确");
            // return UpdateOrUnlock();
            return false;
        }
    }
}
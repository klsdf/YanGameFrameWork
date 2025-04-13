using System;

namespace YanGameFrameWork.AchievementSystem
{
    public abstract class AchievementBase
    {
        /// <summary>
        /// 成就的标题
        /// </summary>
        public string title;
        /// <summary>
        /// 成就的描述
        /// </summary>
        public string description;
        /// <summary>
        /// 成就是否解锁
        /// </summary>
        public bool isUnlock;


        /// <summary>
        /// 成就是否隐藏
        /// </summary>
        public bool isHide;

        /// <summary>
        /// 当成就被隐藏时给的解锁提示
        /// </summary>
        public string unlockHint;

        public AchievementBase(string title, string description, Action onUnlock = null)
        {
            this.OnUnlock = onUnlock;
            this.title = title;
            this.description = description;
            this.isUnlock = false;
        }
        public abstract bool UpdateOrUnlock();
        public abstract bool UpdateOrUnlock(int value);

        public Action OnUnlock;
    }
}
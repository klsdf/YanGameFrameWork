using System;

namespace YanGameFrameWork.AchievementSystem
{
    /// <summary>
    /// 进度型成就
    /// </summary>
    public class ProgressAchievement : AchievementBase
    {
        private int _currentProgress;
        private int _requiredProgress;

        public ProgressAchievement(string title, string description, int requiredProgress, Action onUnlock = null)
            : base(title, description, onUnlock)
        {
            this._currentProgress = 0;
            this._requiredProgress = requiredProgress;
        }

        /// <summary>
        /// 更新或解锁成就
        /// </summary>
        /// <returns>本次是否解锁</returns>
        public override bool UpdateOrUnlock()
        {
            YanGF.Debug.LogWarning(nameof(ProgressAchievement), $"触发了一个进度型成就“{title}”但是没有传入进度值，请检查配置是否正确");
            return false;

        }


        /// <summary>
        /// 更新或解锁成就
        /// </summary>
        /// <param name="value">增加的进度值</param>
        /// <returns>本次是否解锁</returns>
        public override bool UpdateOrUnlock(int value)
        {
            if (isUnlock)
            {
                return false;
            }

            _currentProgress += value;
            if (_currentProgress >= _requiredProgress)
            {
                isUnlock = true;
                OnUnlock?.Invoke();
                return true;
            }
            return false;
        }
    }
}
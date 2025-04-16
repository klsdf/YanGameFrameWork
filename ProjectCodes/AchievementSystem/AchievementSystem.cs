/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-31
 * Description: 成就系统，使用前需要先注册成就，然后才能触发
 *
 * 修改记录：
 * 2025-04-10 闫辰祥 为成就系统添加了队列，分步显示，从而防止多个成就同时触发时互相重叠
 ****************************************************************************/

using YanGameFrameWork.CoreCodes;
using System.Collections.Generic;
using UnityEngine;
// using Sirenix.OdinInspector;
using System;
using System.Collections;
using YanGameFrameWork.Editor;
namespace YanGameFrameWork.AchievementSystem
{
    public class AchievementSystem : Singleton<AchievementSystem>
    {

        [Header("成就弹出的UI")]
        public AchievementPrompt achievementPromptUI;
        private Dictionary<string, AchievementBase> _achievements = new Dictionary<string, AchievementBase>();
        private Queue<AchievementBase> _achievementQueue = new Queue<AchievementBase>();
        private bool _isDisplaying = false;

        public List<AchievementBase> Achievements => new List<AchievementBase>(_achievements.Values);

        private int _achievementDisplayTime = 5;

        /// <summary>
        /// 添加并注册一个成就
        /// </summary>
        /// <param name="achievement">成就</param>
        public void RegisterAchievement(AchievementBase achievement)
        {
            if (!_achievements.ContainsKey(achievement.title))
            {
                _achievements.Add(achievement.title, achievement);
                OnAchievementRegistered?.Invoke(achievement, Achievements);
            }
        }

        /// <summary>
        /// 初始化成就，相当于批量注册成就
        /// </summary>
        /// <param name="achievements">成就列表</param>

        public void InitializeAchievements(List<AchievementBase> achievements)
        {
            _achievements.Clear();
            foreach (var achievement in achievements)
            {
                RegisterAchievement(achievement);
            }
        }


        public List<AchievementBase> GetAllAchievements()
        {
            return new List<AchievementBase>(_achievements.Values);
        }


        /// <summary>
        /// 成就解锁时调用
        /// </summary>
        public Action<AchievementBase> OnAchievementUnlocked;

        /// <summary>
        /// 成就注册时调用
        /// </summary>
        /// <param name="achievement">注册的成就</param>
        /// <param name="achievements">注册后的所有成就</param>
        public Action<AchievementBase, List<AchievementBase>> OnAchievementRegistered;

        /// <summary>
        /// 更新或解锁成就
        /// </summary>
        /// <param name="achievementTitle"></param>
        public void UpdateOrUnlockAchievement(string achievementTitle, int value)
        {
            if (_achievements.TryGetValue(achievementTitle, out var achievement))
            {
                // 本次是否解锁了成就，成就只解锁一次
                bool isUnlockThisTime = achievement.UpdateOrUnlock(value);
                if (isUnlockThisTime)
                {
                    _achievementQueue.Enqueue(achievement);
                    if (!_isDisplaying)
                    {
                        StartCoroutine(DisplayAchievements());
                    }
                }
            }
            else
            {
                YanGF.Debug.LogWarning(nameof(AchievementSystem), "没有找到成就：" + achievementTitle);
            }
        }

        public void UpdateOrUnlockAchievement(string achievementTitle)
        {
            if (_achievements.TryGetValue(achievementTitle, out var achievement))
            {
                // 本次是否解锁了成就，成就只解锁一次
                bool isUnlockThisTime = achievement.UpdateOrUnlock();
                if (isUnlockThisTime)
                {
                    _achievementQueue.Enqueue(achievement);
                    if (!_isDisplaying)
                    {
                        StartCoroutine(DisplayAchievements());
                    }
                }
            }
            else
            {
                YanGF.Debug.LogWarning(nameof(AchievementSystem), "没有找到成就：" + achievementTitle);
            }
        }




        private IEnumerator DisplayAchievements()
        {
            _isDisplaying = true;
            while (_achievementQueue.Count > 0)
            {
                var achievement = _achievementQueue.Dequeue();
                ShowAchievementUI(achievement);
                yield return new WaitForSeconds(_achievementDisplayTime);
            }
            _isDisplaying = false;
        }

        private void ShowAchievementUI(AchievementBase achievement)
        {

            if (achievementPromptUI == null)
            {
                YanGF.Debug.LogWarning(nameof(AchievementSystem), "成就弹出UI为空，看看是不是忘了赋值预制体");
                return;
            }
            Instantiate(achievementPromptUI).GetComponent<AchievementPrompt>().Init(achievement.title, achievement.description);
            OnAchievementUnlocked?.Invoke(achievement);
        }

    }


}
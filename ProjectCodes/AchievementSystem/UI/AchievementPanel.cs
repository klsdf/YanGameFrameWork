/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-07
 * Description: 成就系统的UI部分，用于控制成就面板
 *
 ****************************************************************************/
using YanGameFrameWork.UISystem;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using YanGameFrameWork.AchievementSystem;


/// <summary>
/// 成就的展示面板
/// </summary>
public class AchievementPanel : UIPanelBase
{
    private List<AchievementUItem> _achievementItemList;


    [Header("成就项的预制体")]
    public AchievementUItem achievementItemPrefab;

    [Header("成就项的容器")]
    public Transform container;
    void Start()
    {

        _achievementItemList = new List<AchievementUItem>();

        ClearAchievementList();
        AchievementSystem.Instance.OnAchievementRegistered += AddAchievementItem;
        AchievementSystem.Instance.OnAchievementUnlocked += UpdateAchievementItem;
    }



    /// <summary>
    /// 当成就被新注册的时候，添加成就项
    /// </summary>
    /// <param name="registeredAchievement">注册的成就</param>
    /// <param name="achievements">注册后的所有成就</param>
    void AddAchievementItem(AchievementBase registeredAchievement, List<AchievementBase> achievements)
    {

        AchievementUItem achievementItem = Instantiate(achievementItemPrefab, container).GetComponent<AchievementUItem>();
        achievementItem.Init(registeredAchievement);
        achievementItem.transform.SetParent(container);
        _achievementItemList.Add(achievementItem);


        achievementItem.GetComponent<AchievementUItem>().Init(registeredAchievement);

        YanGF.Debug.Log(nameof(AchievementPanel), $"添加了成就项：{achievementItem.name}");
    }

    void UpdateAchievementItem(AchievementBase achievement)
    {
        // 找到对应的成就项
        AchievementUItem achievementItem = _achievementItemList.Find(item => item.CheckIsSelf(achievement));
        if (achievementItem != null)
        {
            achievementItem.Unlock();
        }
    }


    private void ClearAchievementList()
    {
        _achievementItemList.Clear();
        // 遍历并销毁container下的所有子对象

        if (container != null)
        {
            // 从后往前遍历以避免删除对象时索引变化的问题
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                Destroy(container.GetChild(i).gameObject);
            }
        }
        else
        {
            YanGF.Debug.LogError(nameof(AchievementPanel), "找不到Container对象,请检查UI结构是否正确");
        }
    }



}

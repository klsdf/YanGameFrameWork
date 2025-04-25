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
    private List<AchievementUItem> _achievementItemList = new List<AchievementUItem>();


    [Header("成就项的预制体")]
    public AchievementUItem achievementItemPrefab;

    [Header("成就项的容器")]
    public Transform container;

    [Header("返回按钮")]
    public Button backButton;

    void Start()
    {



        // ClearAchievementList();
        AchievementSystem.Instance.OnAchievementRegistered += AddAchievementItem;
        AchievementSystem.Instance.OnAchievementUnlocked += ShowUnlockItem;

        backButton.onClick.AddListener(() =>
        {
            YanGF.UI.PopPanel();
        });
    }

    public override void OnEnter()
    {
        base.OnEnter();
        ClearAchievementList();
        //拿到现在所有的成就数据进行处理
        List<AchievementBase> achievements = YanGF.Achievement.GetAllAchievements();
        print("当前所有成就数量：" + achievements.Count);
        foreach (var achievement in achievements)
        {
            AddAchievementItem(achievement, achievements);
        }

        UpdateAllAchievementItem();

    }



    /// <summary>
    /// 添加成就的UI项，当成就被注册的时候或者解锁的时候调用
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


    /// <summary>
    /// 更新所有成就项
    /// </summary>
    void UpdateAllAchievementItem()
    {
        foreach (var achievementItem in _achievementItemList)
        {
            achievementItem.TryUnlock();
        }
    }


    /// <summary>
    /// 显示解锁成就，把一个成就的状态更新为已经解锁
    /// </summary>
    /// <param name="achievement">解锁的成就</param>
    void ShowUnlockItem(AchievementBase achievement)
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

/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-07
 * Description: 成就系统的UI部分，用于成就面板中的某一个具体的成就item
 *
 ****************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YanGameFrameWork.AchievementSystem;
public class AchievementUItem : MonoBehaviour
{

    [Header("成就标题")]
    public TMP_Text achievementTitle;

    [Header("成就描述")]
    public TMP_Text achievementDescription;


    private AchievementBase _achievement;

    public void Init(AchievementBase achievement)
    {
        _achievement = achievement;
        achievementTitle.text = YanGF.Localization.GetLocalizedString("未解锁的成就");
        achievementDescription.text = YanGF.Localization.GetLocalizedString("仔细想想哦~");
    }

    public void Unlock()
    {
        _achievement.isUnlock = true;
        achievementTitle.text = YanGF.Localization.GetLocalizedString(_achievement.title);
        achievementDescription.text = YanGF.Localization.GetLocalizedString(_achievement.description);
    }

    public bool CheckIsSelf(AchievementBase achievement)
    {
        if (_achievement == achievement)
        {
            return true;
        }
        return false;
    }
}


/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-03
 * Description: 成就系统的UI部分，来控制弹出的那个成就信息
 *
 ****************************************************************************/
using UnityEngine;
using YanGameFrameWork.UISystem;
using TMPro;
using System.Collections;

public class AchievementPrompt : UIPanelBase
{
    public GameObject achievementItem;
    public TMP_Text achievementTitle;
    public TMP_Text achievementDescription;

    [Header("动画设置")]
    [SerializeField] private float _slideDuration = 0.5f; // 滑动动画时长
    [SerializeField] private float _displayDuration = 3f; // 显示时长

    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    private float _itemHeight;

    private void Awake()
    {
        // 获取成就UI的高度
        RectTransform rectTransform = achievementItem.GetComponent<RectTransform>();
        _itemHeight = rectTransform.rect.height;

        // 设置目标位置为刚好完全显示的位置
        _targetPosition = achievementItem.transform.localPosition;
        // 设置初始位置为屏幕上方完全看不见的位置
        _originalPosition = _targetPosition + Vector3.up * (_itemHeight + 10f); // 额外加10个单位确保完全看不见
    }

    public void Init(string achievementTitle, string achievementDescription)
    {
        this.achievementTitle.text = achievementTitle;
        this.achievementDescription.text = achievementDescription;
        ShowAchievement();
    }


    private void ShowAchievement()
    {
        StartCoroutine(ShowAchievementAnimation());
    }

    private IEnumerator ShowAchievementAnimation()
    {
        // 重置位置到初始位置（屏幕上方）
        achievementItem.transform.localPosition = _originalPosition;

        // 向下滑动
        float elapsedTime = 0f;
        while (elapsedTime < _slideDuration)
        {
            achievementItem.transform.localPosition = Vector3.Lerp(
                _originalPosition,
                _targetPosition,
                elapsedTime / _slideDuration
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 等待显示时间
        yield return new WaitForSeconds(_displayDuration);

        // 向上滑动
        elapsedTime = 0f;
        while (elapsedTime < _slideDuration)
        {
            achievementItem.transform.localPosition = Vector3.Lerp(
                _targetPosition,
                _originalPosition,
                elapsedTime / _slideDuration
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 动画结束后隐藏
        // gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
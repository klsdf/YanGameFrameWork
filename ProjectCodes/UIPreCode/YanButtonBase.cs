/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-28
 * Description: 按钮类
 * 修改记录：
 * 2025-05-28 闫辰祥 
 ****************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using System;


/// <summary>
/// 这个类用于控制开始游戏按钮的行为。
/// </summary>
public abstract class YanButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    
    public Action<PointerEventData> OnClick;
    public Action<PointerEventData> OnMouseEnter;
    public Action<PointerEventData> OnMouseExit;

    /// <summary>
    /// 双击事件，当指针双击时调用。
    /// </summary>
    public Action<PointerEventData> OnDoubleClick;

    // 用于检测双击的时间间隔
    private float _lastClickTime;
    private const float DoubleClickThreshold = 0.3f; // 双击的时间阈值
    



    /// <summary>
    /// 当指针点击时调用。
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - _lastClickTime;

        if (timeSinceLastClick <= DoubleClickThreshold)
        {
            // 触发双击事件
            OnDoubleClick?.Invoke(eventData);
        }
        else
        {
            // 触发单击事件
            if (OnClick != null)
            {
                OnClick(eventData);
            }
            else
            {
                // YanGF.Debug.LogError(nameof(YanButtonBase), $"{gameObject.name}没有设置点击事件");
            }
        }

        _lastClickTime = Time.time;
    }

    /// <summary>
    /// 当指针进入时调用。
    /// </summary>
    public void  OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter?.Invoke(eventData);
    }

    /// <summary>
    /// 当指针退出时调用。
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit?.Invoke(eventData);
    }

}

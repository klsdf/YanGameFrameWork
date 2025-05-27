using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


/// <summary>
/// 这个类用于控制开始游戏按钮的行为。
/// </summary>
public class YanButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{


    public Action OnClick;
    public Action<YanButton> OnMouseEnter;
    public Action<YanButton> OnMouseExit;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit?.Invoke(this);
    }



}

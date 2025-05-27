using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 单选按钮，这个按钮没有组，一个人就是一个组
/// </summary>
[RequireComponent(typeof(Image))]
public class YanSingleToggle : MonoBehaviour, IPointerClickHandler
{
    public Sprite normalSprite;
    public Sprite activeSprite;
    private Image _image;


    private Action<bool> _onToggle;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    protected bool isActive = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        isActive = !isActive;
        _image.sprite = isActive ? activeSprite : normalSprite;
        _onToggle?.Invoke(isActive);
    }

    public void SetOnToggleEvent(Action<bool> onToggle)
    {
        _onToggle = onToggle;
    }

    public void TurnOn()
    {
        isActive = true;
        _image.sprite = activeSprite;
        _onToggle?.Invoke(isActive);
    }

    public void TurnOff()
    {
        isActive = false;
        _image.sprite = normalSprite;
        _onToggle?.Invoke(isActive);
    }

}

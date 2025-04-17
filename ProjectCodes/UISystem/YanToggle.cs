/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-17
 * Description: 一个可以切换状态的按钮，需要来手动继承这个类。
 * 需要设置groupName，只有同一个groupName的按钮才会互相影响。也就是说按下一个的时候，其他同一个groupName的按钮会自动关闭。
 ****************************************************************************/
using UnityEngine.EventSystems;
using UnityEngine;
using System;
public abstract class YanToggle : MonoBehaviour, IPointerClickHandler
{
    public string groupName;



    //自己当前是否处于激活状态
    protected bool isActive = false;

    protected virtual void Start()
    {

        YanGF.Event.AddListener<string, string>("OnButtonClick", OnButtonClick);
    }

    protected virtual void OnDestroy()
    {
        YanGF.Event.RemoveListener<string, string>("OnButtonClick", OnButtonClick);
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        YanGF.Event.TriggerEvent<string, string>("OnButtonClick", groupName, name);
    }



    private void OnButtonClick(string groupName, string name)
    {

        //如果不是自己的组，则不响应
        if (groupName != this.groupName)
        {
            return;
        }

        if (name == this.name)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }
    }







    //////////////////////////////////////////公共方法//////////////////////////////////////////


    /// <summary>
    /// 打开按钮，同时会关闭其他同一个groupName的按钮
    /// </summary>
    public void TurnOn()
    {
        isActive = true;
        OnClickSelf();
    }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    public void TurnOff()
    {
        isActive = false;
        OnClickOther();
    }

    /// <summary>
    /// 点击自身按钮时触发的事件
    /// </summary>
    public Action OnClickSelf;
    /// <summary>
    /// 点击其他按钮时触发的事件
    /// </summary>
    public Action OnClickOther;



}

/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-15
 * Description: 技能节点,也就是技能树的节点，需要子类自己实现
 *
 ****************************************************************************/
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// 技能节点,也就是技能树的节点，可以点击解锁一个技能或者类似的东西
/// </summary>
public abstract class SkillNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button unlockButton;
    public SkillNodeData nodeData;
    public Action OnUnlock;





    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="nodeData"></param>
    public void Init(SkillNodeData nodeData)
    {
        this.nodeData = nodeData;
        nodeData.skillObject = gameObject;
        unlockButton.onClick.AddListener(() =>
        {
            if (nodeData.Parent != null && nodeData.Parent.HasUnlocked == false)
            {
                YanGF.Debug.LogWarning(nameof(SkillNode), $"请先解锁前置技能{nodeData.Parent.Name}");
                return;
            }
            if (nodeData.Condition() && !nodeData.HasUnlocked)
            {
                nodeData.OnUnlock(gameObject);


                nodeData.HasUnlocked = true;

                //一旦某个节点更新，会刷新整棵树
                nodeData.SkillSystem.UpdateDisplay();
                Save();
            }
        });
    }



    /// <summary>
    /// 当解锁了该节点后，会自动调用这个函数，用来存储解锁后的数据，当然了，也可以不实现 ，但是强烈建议实现，所以使用了抽象方法
    /// </summary>
    public abstract void Save();



    public void UpdateDisplay()
    {
        //有父节点，并且父节点未解锁时，卡牌显示锁定状态
        if (nodeData.Parent != null && nodeData.Parent.HasUnlocked == false)
        {
            ShowLockInfo();
            return;
        }
        if (nodeData.HasUnlocked)
        {
            ShowUnlockInfo();
        }
        else
        {
            ShowCanUnlockInfo();
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPosition
        );



        Vector3 worldPosition2 = rectTransform.position; // 获取RectTransform的世界坐标
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition2); // 转换为屏幕坐标

        Debug.Log($"Screen Position: {screenPosition}");



        nodeData.SkillSystem.ShowPromptPop(nodeData, worldPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        YanGF.UI.PopPanel<SkillPromptPop>();
    }


    /////////////////////////////虚方法////////////////////////////////////////


    /// <summary>
    /// 在场景中初始化，也就是直接在场景中生成一个技能节点
    /// </summary>
    /// <param name="nodeData"></param>
    public virtual void InitInScene(SkillNodeData nodeData)
    {

        gameObject.name = nodeData.Name;
    }



    /////////////////////////////////子类需要实现的方法////////////////////////////////


    /// <summary>
    /// 当技能节点被锁定时，显示的信息
    /// </summary>
    protected abstract void ShowLockInfo();


    /// <summary>
    /// 当技能节点可以解锁时，显示的信息
    /// </summary>
    protected abstract void ShowCanUnlockInfo();

    /// <summary>
    /// 当技能节点被解锁时，显示的信息
    /// </summary>
    protected abstract void ShowUnlockInfo();
}
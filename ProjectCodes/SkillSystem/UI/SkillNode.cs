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
                nodeData.SkillSystem.UpdateDisplay();
            }
        });

    }


    /// <summary>
    /// 在场景中初始化，也就是直接在场景中生成一个技能节点
    /// </summary>
    /// <param name="nodeData"></param>
    public virtual void InitInScene(SkillNodeData nodeData)
    {

        gameObject.name = nodeData.Name;
    }



    public void UpdateDisplay()
    {
        if (nodeData.Parent != null && nodeData.Parent.HasUnlocked == false)
        {
            ShowLockInfo();

            return;
        }
        ShowLockInfo();
        // nameText.text = nodeData.Name;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        // 获取 SkillNode 的 RectTransform
        // RectTransform skillNodeRect = GetComponent<RectTransform>();
        nodeData.SkillSystem.ShowPromptPop(nodeData, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        YanGF.UI.PopElement();
    }





    /////////////////////////////////子类需要实现的方法////////////////////////////////


    /// <summary>
    /// 当技能节点被锁定时，显示的信息
    /// </summary>
    protected abstract void ShowLockInfo();

    /// <summary>
    /// 当技能节点被解锁时，显示的信息
    /// </summary>
    protected abstract void ShowUnlockInfo();
}
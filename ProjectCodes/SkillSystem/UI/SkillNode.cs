/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-15
 * Description: 技能节点,也就是技能树的节点
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
public class SkillNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text nameText;
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


    public void InitInScene(SkillNodeData nodeData)
    {
        nameText.text = nodeData.Name;
        gameObject.name = nodeData.Name;
    }



    public void UpdateDisplay()
    {
        if (nodeData.Parent != null && nodeData.Parent.HasUnlocked == false)
        {
            nameText.text = "???";
            return;
        }
        nameText.text = nodeData.Name;
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

}
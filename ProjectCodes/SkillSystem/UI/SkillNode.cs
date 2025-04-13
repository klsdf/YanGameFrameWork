using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        // SkillPromptPop skillPromptPop = YanGF.UI.PushElement<SkillPromptPop>() as SkillPromptPop;
        // skillPromptPop.ShowSkillPrompt(nodeData);

        // // 获取 SkillNode 的 RectTransform
        // RectTransform skillNodeRect = GetComponent<RectTransform>();
        // // 获取 SkillPromptPop 的 RectTransform
        // RectTransform skillPromptRect = skillPromptPop.GetComponent<RectTransform>();

        // // 计算新位置
        // Vector3 newPosition = skillNodeRect.position;
        // newPosition.y += skillNodeRect.rect.height + skillPromptRect.rect.height + 100;

        // // 设置 SkillPromptPop 的位置
        // skillPromptRect.position = newPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        YanGF.UI.PopElement();
    }

}
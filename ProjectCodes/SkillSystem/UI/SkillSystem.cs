/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-15
 * Description: 技能系统,需要子类重写
 *
 ****************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using YanGameFrameWork.Editor;
#endif

public abstract class SkillSystem : MonoBehaviour
{

    [Header("技能树的UI预制体")]
    public SkillNode skillButtonPrefab; // 预制体

    [Header("技能树UI的容器")]
    public Transform container; // 场景中的Canvas

    [Header("技能提示弹窗")]
    public SkillPromptPop skillPromptPopPrefab;

    void Start()
    {
        rootSkillList = CreateInitData();
        InitLines();
        UpdateDisplay();
    }


    /// <summary>
    /// 初始化技能节点
    /// </summary>
    /// <param name="name">技能名称</param>
    /// <param name="description">技能描述</param>
    /// <param name="onUnlock">技能解锁时执行的事件</param>
    /// <param name="condition">技能解锁的条件</param>
    /// <returns></returns>
    protected SkillNodeData InitNode(SkillNodeData nodedata)
    {
        nodedata.SkillSystem = this;
        // SkillNodeData nodedata = new SkillNodeData(
        //     name: name,
        //     description: description,
        //     onUnlock: onUnlock,
        //     condition: condition,
        //     skillSystem: this
        // );
        if (Application.isPlaying)
        {
            GameObject skillObject = GameObject.Find(nodedata.Name);
            SkillNode skillNode = skillObject.GetComponent<SkillNode>();
            skillNode.Init(nodedata);

        }
        return nodedata;
    }

    public List<SkillNodeData> rootSkillList = new List<SkillNodeData>();



    /// <summary>
    /// 创建技能树的初始数据，需要子类重写
    /// </summary>
    /// <returns></returns>
    public abstract List<SkillNodeData> CreateInitData();

    /// <summary>
    /// 初始化技能节点的显示
    /// </summary>
    public void UpdateDisplay()
    {
        List<SkillNodeData> flattenedSkillTree = FlattenSkillTree(rootSkillList[0]);
        foreach (SkillNodeData skillNodeData in flattenedSkillTree)
        {
            skillNodeData.skillObject.GetComponent<SkillNode>().UpdateDisplay();
        }
    }


    /// <summary>
    /// 显示技能提示弹窗
    /// </summary>
    /// <param name="skillNodeData">技能节点数据</param>
    /// <param name="nodePosition">技能节点位置</param>
    public void ShowPromptPop(SkillNodeData skillNodeData, Vector3 nodePosition)
    {
        if (skillPromptPopPrefab == null)
        {
            YanGF.Debug.LogWarning(nameof(SkillSystem), "技能提示弹窗的预制体为空");
            return;
        }
        SkillPromptPop skillPromptPop = YanGF.UI.PushElement(skillPromptPopPrefab) as SkillPromptPop;

        var newPosition = nodePosition;
        newPosition.y += skillPromptPop.GetComponent<RectTransform>().rect.height + 50;
        skillPromptPop.ShowSkillPrompt(skillNodeData, newPosition);
    }

    private void InitLines()
    {
        List<SkillNodeData> flattenedSkillTree = FlattenSkillTree(rootSkillList[0]);
        foreach (SkillNodeData skillNodeData in flattenedSkillTree)
        {
            RectTransform parentTransform = container.GetComponent<RectTransform>();
            RectTransform startTransform = skillNodeData.skillObject.GetComponent<RectTransform>();

            foreach (SkillNodeData child in skillNodeData.Children)
            {
                RectTransform endTransform = child.skillObject.GetComponent<RectTransform>();
                DrawLine(parentTransform, startTransform.anchoredPosition, endTransform.anchoredPosition, 2f, Color.white);
            }
        }
    }


    /// <summary>
    /// 绘制技能树的线
    /// </summary>
    /// <param name="parent">父级</param>
    /// <param name="start">起始点</param>
    /// <param name="end">结束点</param>
    /// <param name="width">线宽</param>
    /// <param name="color">颜色</param>
    private void DrawLine(RectTransform parent, Vector2 start, Vector2 end, float width, Color color)
    {
        GameObject line = new GameObject("Line");
        line.transform.SetParent(parent, false);
        Image image = line.AddComponent<Image>();
        image.color = color;

        RectTransform rectTransform = line.GetComponent<RectTransform>();
        Vector2 direction = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        rectTransform.sizeDelta = new Vector2(distance, width);
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.anchoredPosition = start;
        rectTransform.rotation = Quaternion.FromToRotation(Vector2.right, direction);
    }


    /// <summary>
    /// 扁平化技能树
    /// </summary>
    /// <param name="node">这里是根节点！！会自动扁平化的</param>
    /// <returns></returns>
    private List<SkillNodeData> FlattenSkillTree(SkillNodeData node)
    {
        List<SkillNodeData> result = new List<SkillNodeData>();
        result.Add(node);
        foreach (SkillNodeData child in node.Children)
        {
            result.AddRange(FlattenSkillTree(child));
        }
        return result;
    }


    #region 编辑器方法

#if UNITY_EDITOR
    /// <summary>
    /// 生成场景的对象
    /// </summary>
    [Button("生成场景的对象")]
    public void CreateSkillNodesToScene()
    {

        List<SkillNodeData> rootSkillList = CreateInitData();
        List<SkillNodeData> flattenedSkillTree = FlattenSkillTree(rootSkillList[0]);
        ClearRepeatSkillNodes(flattenedSkillTree);
        CreateSkillNodes(flattenedSkillTree);
        YanGF.Debug.Log(nameof(SkillSystem), "成功生成场景的对象");
    }


    /// <summary>
    /// 清除编辑器中重复的技能节点
    /// </summary>
    /// <param name="skillLists">注意！这里是扁平化后的技能列表！！</param>
    private void ClearRepeatSkillNodes(List<SkillNodeData> skillLists)
    {


        // 创建一个HashSet来存储rootSkillList中的技能名称
        HashSet<string> skillNames = new HashSet<string>();
        foreach (SkillNodeData skillNodeData in skillLists)
        {
            skillNames.Add(skillNodeData.Name);
        }

        // 遍历container中的所有子对象
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            // 如果子对象的名称不在skillNames中，则删除该子对象
            if (!skillNames.Contains(child.name))
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    /// <summary>
    /// 在场景中创建技能节点
    /// </summary>
    /// <param name="skillNodeDataArray">注意！这里是扁平化后的技能列表！！</param>
    private void CreateSkillNodes(List<SkillNodeData> skillNodeDataArray)
    {
        foreach (SkillNodeData skillNodeData in skillNodeDataArray)
        {
            if (GameObject.Find(skillNodeData.Name) != null)
            {
                continue;
            }
            SkillNode skillButton = PrefabUtility.InstantiatePrefab(skillButtonPrefab, container.transform) as SkillNode;
            skillButton.InitInScene(skillNodeData);

            // 设置随机位置
            float randomX = UnityEngine.Random.Range(-400f, 400f);
            float randomY = UnityEngine.Random.Range(-300f, 300f);

            RectTransform rectTransform = skillButton.gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(randomX, randomY);
        }
    }
#endif
    #endregion

}
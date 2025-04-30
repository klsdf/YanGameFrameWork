/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-15
 * Description: 技能系统,需要子类重写
 *
 * 修改记录：
 * 2025-04-30 闫辰祥 技能提示弹窗的显示位置优化，会自动寻找一个能放得下的地方
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


    /// <summary>
    /// 技能树的根节点，因为技能树的根节点不一定只有一个，所以需要一个列表来存储
    /// </summary>
    public List<SkillNodeData> rootSkillList = new List<SkillNodeData>();

    /// <summary>
    /// 已经解锁的技能节点
    /// </summary>
    public List<SkillNodeData> AllSkillNodeDatas
    {
        get
        {
            return FlattenAndMergeSkillTrees(rootSkillList);
        }
    }

    void Start()
    {
        rootSkillList = CreateInitData();
        InitLines();



        //从本地数据中加载已经解锁的技能名称
        List<string> unlockedSkillNames = LoadUnlockedSkillName();
        foreach (string unlockedSkillName in unlockedSkillNames)
        {
            SkillNodeData skillNodeData = AllSkillNodeDatas.Find(skill => skill.Name == unlockedSkillName);
            if (skillNodeData != null)
            {
                skillNodeData.HasUnlocked = true;
            }
        }

        UpdateDisplay();
    }


    /// <summary>
    /// 从本地数据中加载技能数据，需要配合SkillNode的save方法来使用，具体的节点解锁时来保存数据，在初始化的时候，SkillSystem会自动读取数据
    /// 具体内容就是，把已经解锁的节点的nodeData.HasUnlocked设置为true
    /// </summary>
    abstract protected List<string> LoadUnlockedSkillName();


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






    /// <summary>
    /// 创建技能树的初始数据，需要子类重写
    /// </summary>
    /// <returns></returns>
    public abstract List<SkillNodeData> CreateInitData();

    /// <summary>
    /// 刷新技能节点的显示
    /// </summary>
    public void UpdateDisplay()
    {
        List<SkillNodeData> flattenedSkillTree = FlattenAndMergeSkillTrees(rootSkillList);
        foreach (SkillNodeData skillNodeData in flattenedSkillTree)
        {
            skillNodeData.skillObject.GetComponent<SkillNode>().UpdateDisplay();
        }
    }




    public Vector3 nodePosition;
    public float worldHeight;
    public float worldWidth;
    const float OFFSET_Y = 0; // 正值以便在上方显示
    const float OFFSET_X = 0; // 正值以便在左边显示

    /// <summary>
    /// 显示技能提示弹窗
    /// </summary>
    /// <param name="skillNodeData">技能节点数据</param>
    /// <param name="nodePosition">技能节点位置</param>
    public void ShowPromptPop(SkillNodeData skillNodeData, Vector3 nodePosition)
    {
        this.nodePosition = nodePosition;
        if (skillPromptPopPrefab == null)
        {
            YanGF.Debug.LogWarning(nameof(SkillSystem), "技能提示弹窗的预制体为空");
            return;
        }

        SkillPromptPop skillPromptPop = YanGF.UI.PushElement(skillPromptPopPrefab) as SkillPromptPop;

        RectTransform rectTransform = skillPromptPop.GetComponent<RectTransform>();
        Vector3 newPosition = FindBestPosition(rectTransform, nodePosition);
        skillPromptPop.ShowSkillPrompt(skillNodeData, newPosition);
    }



    /// <summary>
    /// 寻找一个能放得下技能描述弹窗的位置
    /// </summary>
    /// <param name="rect">技能提示弹窗的矩形</param>
    /// <param name="nodePosition">技能节点位置，使用世界坐标</param>
    private Vector3 FindBestPosition(RectTransform rectTransform, Vector3 nodePosition)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            YanGF.Debug.LogWarning(nameof(SkillSystem), "主摄像机未找到");
            return nodePosition;
        }

        float screenHeight = Screen.height;
        float screenWidth = Screen.width;


        // 计算世界坐标系下的高度
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);
        worldHeight = Vector3.Distance(worldCorners[0], worldCorners[1]);
        worldWidth = Vector3.Distance(worldCorners[0], worldCorners[3]);

        // 判断是否应该显示在上面

        print("屏幕坐标系下的nodePosition:" + Camera.main.WorldToScreenPoint(nodePosition));

        Vector3 topWorldPoint = nodePosition + Vector3.up * worldHeight + Vector3.up * OFFSET_Y;

        if (Camera.main.WorldToScreenPoint(topWorldPoint + Vector3.up * worldHeight / 2).y < screenHeight)
        {
            print("上面点在屏幕内:" + Camera.main.WorldToScreenPoint(topWorldPoint + Vector3.up * worldHeight / 2));
            return topWorldPoint;
        }
        else
        {
            print("上面点不在屏幕内:" + Camera.main.WorldToScreenPoint(topWorldPoint + Vector3.up * worldHeight / 2));
        }



        // 判断是否应该显示在下面
        Vector3 bottomWorldPoint = nodePosition + Vector3.down * worldHeight + Vector3.down * OFFSET_Y;
        if (Camera.main.WorldToScreenPoint(bottomWorldPoint + Vector3.down * worldHeight / 2).y > 0)
        {
            print("下面点在屏幕内:" + Camera.main.WorldToScreenPoint(bottomWorldPoint + Vector3.down * worldHeight / 2));
            return bottomWorldPoint;
        }
        else
        {
            print("下面点不在屏幕内:" + Camera.main.WorldToScreenPoint(bottomWorldPoint + Vector3.down * worldHeight / 2));
        }

        // 判断是否应该显示在左边
        Vector3 leftWorldPoint = nodePosition + Vector3.left * worldWidth + Vector3.left * OFFSET_X;
        if (Camera.main.WorldToScreenPoint(leftWorldPoint + Vector3.left * worldWidth / 2).x > 0)
        {
            print("左边点在屏幕内:" + Camera.main.WorldToScreenPoint(leftWorldPoint + Vector3.left * worldWidth / 2));
            return leftWorldPoint;
        }
        else
        {
            print("左边点不在屏幕内:" + Camera.main.WorldToScreenPoint(leftWorldPoint + Vector3.left * worldWidth / 2));
        }

        // 判断是否应该显示在右边
        Vector3 rightWorldPoint = nodePosition + Vector3.right * worldWidth + Vector3.right * OFFSET_X;
        if (Camera.main.WorldToScreenPoint(rightWorldPoint + Vector3.right * worldWidth / 2).x < screenWidth)
        {
            print("右边点在屏幕内:" + Camera.main.WorldToScreenPoint(rightWorldPoint + Vector3.right * worldWidth / 2));
            return rightWorldPoint;
        }
        else
        {
            print("右边点不在屏幕内:" + Camera.main.WorldToScreenPoint(rightWorldPoint + Vector3.right * worldWidth / 2));
        }

        // 确保返回的坐标是世界坐标
        return nodePosition;
    }






    private void InitLines()
    {
        List<SkillNodeData> flattenedSkillTree = FlattenAndMergeSkillTrees(rootSkillList);
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
    private void DrawLine(RectTransform parent, Vector2 start, Vector2 end, float width, Color color, int siblingIndex = 0)
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

        // 设置线条对象在父节点中的位置
        line.transform.SetSiblingIndex(siblingIndex);
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


    /// <summary>
    /// 扁平化多个技能树并连接它们
    /// </summary>
    /// <param name="nodeLists">多个技能树的根节点列表</param>
    /// <returns>扁平化后的技能节点列表</returns>
    private List<SkillNodeData> FlattenAndMergeSkillTrees(List<SkillNodeData> nodeLists)
    {
        List<SkillNodeData> result = new List<SkillNodeData>();
        foreach (var rootNode in nodeLists)
        {
            result.AddRange(FlattenSkillTree(rootNode));
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

    void OnDrawGizmos()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        // 计算四个点的世界坐标
        Vector3 topWorldPoint = nodePosition + Vector3.up * worldHeight + Vector3.up * OFFSET_Y;
        Vector3 bottomWorldPoint = nodePosition + Vector3.down * worldHeight + Vector3.down * OFFSET_Y;
        Vector3 leftWorldPoint = nodePosition + Vector3.left * worldWidth + Vector3.left * OFFSET_X;
        Vector3 rightWorldPoint = nodePosition + Vector3.right * worldWidth + Vector3.right * OFFSET_X;

        // 将世界坐标转换为屏幕坐标
        Vector3 topScreenPoint = mainCamera.WorldToScreenPoint(topWorldPoint);
        Vector3 bottomScreenPoint = mainCamera.WorldToScreenPoint(bottomWorldPoint);
        Vector3 leftScreenPoint = mainCamera.WorldToScreenPoint(leftWorldPoint);
        Vector3 rightScreenPoint = mainCamera.WorldToScreenPoint(rightWorldPoint);

        // 设置Gizmos的颜色
        Gizmos.color = Color.red;

        // 绘制四个点的区域
        Gizmos.DrawSphere(topScreenPoint, 0.1f);
        Gizmos.DrawSphere(bottomScreenPoint, 0.1f);
        Gizmos.DrawSphere(leftScreenPoint, 0.1f);
        Gizmos.DrawSphere(rightScreenPoint, 0.1f);
    }

}

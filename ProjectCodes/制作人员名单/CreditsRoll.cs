using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 制作人员名单滚动控制器
/// 支持多种类型的内容渲染和自定义滚动效果
/// </summary>
public class CreditsRoll : MonoBehaviour
{
    [Header("UI组件")]
    [Tooltip("内容区的RectTransform")]
    public RectTransform contentArea;
    
    [Header("预制体配置")]
    [Tooltip("文本预制体")]
    public GameObject textPrefab;
    [Tooltip("头像预制体")]
    public GameObject avatarPrefab;
    [Tooltip("分隔线预制体")]
    public GameObject dividerPrefab;
    
    [Header("滚动设置")]
    [Tooltip("滚动速度（像素/秒）")]
    public float scrollSpeed = 50f;
    [Tooltip("是否循环播放")]
    public bool loop = true;
    [Tooltip("播放完成后是否自动隐藏")]
    public bool autoHide = true;
    
    [Header("制作人员名单数据")]
    [SerializeField]
    private List<CreditsRollData> creditsData = new List<CreditsRollData>();
    
    // 私有变量
    private Vector2 initialPosition;
    private List<ICreditsRenderer> activeRenderers = new List<ICreditsRenderer>();
    private bool isPlaying = false;
    private Coroutine scrollCoroutine;
    
    /// <summary>
    /// 初始化
    /// </summary>
    private void Start()
    {
        if (contentArea == null)
        {
            Debug.LogError("请设置内容区域");
            return;
        }
        
        initialPosition = contentArea.anchoredPosition;
        SetupDefaultCredits();
    }
    
    /// <summary>
    /// 设置默认的制作人员名单
    /// </summary>
    private void SetupDefaultCredits()
    {
        if (creditsData.Count == 0)
        {
            creditsData = new List<CreditsRollData>
            {
                new CreditsTitleData("制作人员名单", Color.yellow, 36, 3f),
                new CreditsDividerData(30f, Color.gray, 1f),
                new CreditsTextData("游戏制作", Color.white, 28, 2f),
                new CreditsAvatarData("张三", "程序", "Avatars/Programmer", null, 3f),
                new CreditsAvatarData("李四", "美术", "Avatars/Artist", null, 3f),
                new CreditsAvatarData("王五", "策划", "Avatars/Designer", null, 3f),
                new CreditsDividerData(20f, Color.gray, 1f),
                new CreditsTextData("特别感谢", Color.cyan, 24, 2f),
                new CreditsTextData("感谢所有支持我们的玩家", Color.white, 20, 2f),
            };
        }
    }
    
    /// <summary>
    /// 开始播放制作人员名单
    /// </summary>
    public void PlayCredits()
    {
        if (isPlaying) return;
        
        isPlaying = true;
        gameObject.SetActive(true);
        GenerateCredits();
        
        if (scrollCoroutine != null)
            StopCoroutine(scrollCoroutine);
        scrollCoroutine = StartCoroutine(ScrollCoroutine());
    }
    
    /// <summary>
    /// 停止播放
    /// </summary>
    public void StopCredits()
    {
        isPlaying = false;
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
            scrollCoroutine = null;
        }
        
        if (autoHide)
            gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 重置到初始位置
    /// </summary>
    public void ResetPosition()
    {
        contentArea.anchoredPosition = initialPosition;
    }
    
    /// <summary>
    /// 生成制作人员名单
    /// </summary>
    private void GenerateCredits()
    {
        // 清理现有内容
        ClearContent();
        
        // 生成新内容
        foreach (var data in creditsData)
        {
            GameObject prefab = GetPrefabForDataType(data);
            if (prefab != null)
            {
                var renderer = CreditsRendererFactory.CreateRenderer(data, prefab, contentArea);
                if (renderer != null)
                {
                    activeRenderers.Add(renderer);
                }
            }
        }
        
        // 重新布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentArea);
    }
    
    /// <summary>
    /// 根据数据类型获取对应的预制体
    /// </summary>
    private GameObject GetPrefabForDataType(CreditsRollData data)
    {
        if (data is CreditsTextData || data is CreditsTitleData)
            return textPrefab;
        else if (data is CreditsAvatarData)
            return avatarPrefab;
        else if (data is CreditsDividerData)
            return dividerPrefab;
        
        return textPrefab; // 默认使用文本预制体
    }
    
    /// <summary>
    /// 清理内容
    /// </summary>
    private void ClearContent()
    {
        // 清理渲染器
        foreach (var renderer in activeRenderers)
        {
            renderer?.Clear();
        }
        activeRenderers.Clear();
        
        // 清理子物体
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }
    }
    
    /// <summary>
    /// 滚动协程
    /// </summary>
    private IEnumerator ScrollCoroutine()
    {
        ResetPosition();
        
        while (isPlaying)
        {
            // 向上滚动
            contentArea.anchoredPosition += Vector2.up * scrollSpeed * Time.unscaledDeltaTime;
            
            // 检查是否滚动到底部
            if (IsScrolledToBottom())
            {
                if (loop)
                {
                    ResetPosition();
                }
                else
                {
                    StopCredits();
                    break;
                }
            }
            
            yield return null;
        }
    }
    
    /// <summary>
    /// 检查是否滚动到底部
    /// </summary>
    private bool IsScrolledToBottom()
    {
        float contentHeight = contentArea.rect.height;
        float parentHeight = ((RectTransform)contentArea.parent).rect.height;
        float currentY = contentArea.anchoredPosition.y;
        
        return currentY >= contentHeight + parentHeight;
    }
    
    /// <summary>
    /// 添加制作人员数据
    /// </summary>
    public void AddCreditsData(CreditsRollData data)
    {
        creditsData.Add(data);
    }
    
    /// <summary>
    /// 清空制作人员数据
    /// </summary>
    public void ClearCreditsData()
    {
        creditsData.Clear();
    }
    
    /// <summary>
    /// 设置制作人员数据
    /// </summary>
    public void SetCreditsData(List<CreditsRollData> data)
    {
        creditsData = new List<CreditsRollData>(data);
    }
    
    /// <summary>
    /// 获取当前制作人员数据
    /// </summary>
    public List<CreditsRollData> GetCreditsData()
    {
        return new List<CreditsRollData>(creditsData);
    }
}

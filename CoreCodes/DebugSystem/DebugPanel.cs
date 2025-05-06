using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
/// <summary>
/// 自定义的Debug面板，用于在打包后的版本中显示Debug信息。
/// </summary>
public class DebugPanel : MonoBehaviour
{
    [Header("DebugItem预制体")]
    public GameObject debugItemPrefab;

    [Header("DebugItem容器")]
    public Transform debugItemContainer;

    [Header("堆栈跟踪文本")]
    public TMP_Text stackTraceText;

    [Header("清除按钮")]
    public Button clearButton;

    [Header("合并按钮")]
    public Button mergeButton;


    private bool isMerged = false;


    private DebugItemBucket debugItemBucket = new DebugItemBucket();

    void Awake()
    {
        // 注册Debug日志回调
        Application.logMessageReceived += HandleLog;

        // 为清除按钮注册点击事件
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(Clear);
        }

        // 为合并按钮注册点击事件
        if (mergeButton != null)
        {
            mergeButton.onClick.AddListener(ToggleMerge);
        }
    }

    void OnDestroy()
    {
        // 注销Debug日志回调
        Application.logMessageReceived -= HandleLog;

        // 注销清除按钮的点击事件
        if (clearButton != null)
        {
            clearButton.onClick.RemoveListener(Clear);
        }

        // 注销合并按钮的点击事件
        if (mergeButton != null)
        {
            mergeButton.onClick.RemoveListener(ToggleMerge);
        }
    }

    /// <summary>
    /// 处理日志信息，将其显示在Debug面板上。
    /// </summary>
    /// <param name="logString">日志信息</param>
    /// <param name="stackTrace">堆栈跟踪</param>
    /// <param name="type">日志类型</param>
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 创建新的DebugItem
        GameObject debugItem = Instantiate(debugItemPrefab, debugItemContainer);
        DebugItem debugItemScript = debugItem.GetComponent<DebugItem>();
        // 初始化DebugItem，传递日志信息和堆栈跟踪
        debugItemScript.Init(logString, stackTrace, type, this);

        // 将DebugItem添加到桶中
        debugItemBucket.AddItem(logString, debugItemScript);
    }

    public void ShowStackTrace(string stackTrace)
    {
        stackTraceText.text = stackTrace;
    }

    /// <summary>
    /// 清除所有日志信息。
    /// </summary>
    public void Clear()
    {
        // 清空Debug项容器中的所有子对象
        foreach (Transform child in debugItemContainer)
        {
            Destroy(child.gameObject);
        }
        debugItemBucket.Clear();

        // 清空堆栈跟踪文本
        stackTraceText.text = "";
    }



    public void ToggleMerge()
    {
        if (isMerged)
        {
            UnMerge();
        }
        else
        {
            Merge();
        }
    }




    /// <summary>
    /// 合并相同的日志信息。
    /// </summary>
    public void Merge()
    {
        List<string> logStrings = debugItemBucket.GetAllLogStrings();
        foreach (var logString in logStrings)
        {
            var items = debugItemBucket.GetItems(logString);
            bool isFirst = true;
            foreach (var item in items)
            {
                if (isFirst)
                {
                    isFirst = false;
                    item.SetMergeCount(items.Count);
                    continue;
                }
                // 设置除了第一个以外的DebugItem为不活跃
                item.gameObject.SetActive(false);
            }
        }
        isMerged = true;
    }

    public void UnMerge()
    {
        List<string> logStrings = debugItemBucket.GetAllLogStrings();
        foreach (var logString in logStrings)
        {
            var items = debugItemBucket.GetItems(logString);
            foreach (var item in items)
            {
                item.gameObject.SetActive(true);
                item.SetNotMerge();
            }
        }

        isMerged = false;
    }
}

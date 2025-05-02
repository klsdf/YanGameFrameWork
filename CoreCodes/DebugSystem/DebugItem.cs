using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 表示一个Debug项，显示日志信息和堆栈跟踪。
/// </summary>
public class DebugItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // 显示日志信息的Text组件
    [Header("日志信息")]
    public TMP_Text logText;

    // 显示合并数量的Text组件
    [Header("合并数量")]
    public TMP_Text mergeCountText;

    // 存储堆栈跟踪信息
    private string stackTrace;

    private DebugPanel debugPanel;

    // 背景Image组件
    private Image background;

    // 原始背景颜色
    private Color originalColor;

    // 悬停时的背景颜色
    public Color hoverColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    // 日志类型
    private LogType logType;



    /// <summary>
    /// 获取日志文本。
    /// </summary>
    public string LogText => logText.text;


    void Awake()
    {
        // 获取背景Image组件
        background = GetComponent<Image>();
        if (background != null)
        {
            originalColor = background.color;
        }
    }

    /// <summary>
    /// 初始化DebugItem。
    /// </summary>
    /// <param name="logString">日志信息</param>
    /// <param name="stackTrace">堆栈跟踪</param>
    public void Init(string logString, string stackTrace, LogType logType, DebugPanel debugPanel)
    {
        logText.text = logString;
        this.stackTrace = stackTrace;
        this.logType = logType;
        this.debugPanel = debugPanel;

        SetNotMerge();
    }


    public void SetMergeCount(int count)
    {
        mergeCountText.enabled = true;
        mergeCountText.text = $"x{count}";
    }


    public void SetNotMerge()
    {
        mergeCountText.enabled = false;
    }

    /// <summary>
    /// 当点击DebugItem时调用，显示堆栈跟踪。
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        debugPanel.ShowStackTrace(stackTrace);
    }

    /// <summary>
    /// 当鼠标进入时调用，改变背景颜色。
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (background != null)
        {
            background.color = hoverColor;
        }
    }

    /// <summary>
    /// 当鼠标离开时调用，恢复背景颜色。
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (background != null)
        {
            background.color = originalColor;
        }
    }



}

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))] // 确保有碰撞体
public class SelectableUnit : MonoBehaviour
{
    [Header("Selection Settings")]
    [SerializeField] private bool isSelected = false;
    [SerializeField] private SpriteRenderer selectionIndicator; // 选择指示器（如高亮边框）
    [SerializeField] public bool isPreselected = false;

    [Header("Events")]
    public UnityEvent onSelected;      // 选中时触发的事件
    public UnityEvent onDeselected;    // 取消选中时触发的事件
    public UnityEvent<Vector3> onRightClick; // 右键命令事件
    public UnityEvent onPreselected;
    public UnityEvent onUnpreselected;

    private Color originalColor; // 原始颜色缓存

    private void Awake()
    {
        // 初始化选择指示器
        if (selectionIndicator != null)
        {
            originalColor = selectionIndicator.color;
            selectionIndicator.gameObject.SetActive(false);
        }
    }

    #region Selection Control
    /// <summary>
    /// 选中单位
    /// </summary>
    public void Select()
    {
        if (isSelected) return;

        isSelected = true;
        onSelected?.Invoke();

        print($"Unit {name} selected");

        GetComponent<SpriteRenderer>().color = Color.red;
    }

    /// <summary>
    /// 取消选中单位
    /// </summary>
    public void Deselect()
    {
        if (!isSelected) return;

        isSelected = false;
        onDeselected?.Invoke();

        print($"Unit {name} deselected");

        GetComponent<SpriteRenderer>().color = Color.white;
    }

    /// <summary>
    /// 切换选中状态
    /// </summary>
    public void ToggleSelection()
    {
        if (isSelected) Deselect();
        else Select();
    }


    /// <summary>
    /// 预选中
    /// </summary>
    public void Preselect()
    {
        if (isPreselected || isSelected) return;
        isPreselected = true;


        GetComponent<SpriteRenderer>().color = Color.green;
        onPreselected?.Invoke();
    }


    /// <summary>
    /// 取消预选中
    /// </summary>
    public void Unpreselect()
    {
        if (!isPreselected || isSelected) return;
        isPreselected = false;

        GetComponent<SpriteRenderer>().color = Color.white;
        onUnpreselected?.Invoke();
    }
    #endregion

    #region Visual Feedback



    /// <summary>
    /// 鼠标离开效果
    /// </summary>
    private void OnMouseExit()
    {
        if (selectionIndicator != null && !isSelected)
        {
            selectionIndicator.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Command Handling
    /// <summary>
    /// 处理右键命令
    /// </summary>
    public void HandleRightClick(Vector3 worldPosition)
    {
        if (!isSelected) return;

        Debug.Log($"Unit {name} received command at position: {worldPosition}");
        onRightClick?.Invoke(worldPosition);

        // 这里可以添加移动逻辑或其他命令响应
        // 例如: GetComponent<UnitMovement>()?.MoveTo(worldPosition);
    }
    #endregion

    // 保持与旧代码兼容的别名方法
    public void Controled() => Select();
    public void NotControl() => Deselect();
    public void SetMouseRightButtonDown(Vector3 mousePosition) => HandleRightClick(Camera.main.ScreenToWorldPoint(mousePosition));
}
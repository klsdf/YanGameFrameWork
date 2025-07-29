using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionController : MonoBehaviour
{
    [Header("Selection Settings")]
    [SerializeField] private Transform selectionArea; // 选择区域Sprite的Transform
    [SerializeField] private Color selectionColor = new Color(0.8f, 0.8f, 0.95f, 0.25f);
    [SerializeField] private Color borderColor = new Color(0.8f, 0.8f, 1f, 0.8f);
    [SerializeField] private float borderWidth = 2f;

    private Vector3 selectionStartPosition;
    private List<GameObject> selectedUnits = new List<GameObject>();
    private SpriteRenderer selectionSpriteRenderer;
    private MaterialPropertyBlock propertyBlock;
    private HashSet<SelectableUnit> preselectedUnits = new HashSet<SelectableUnit>();

    private void Awake()
    {
        // 初始化选择区域
        selectionArea.gameObject.SetActive(false);
        selectionSpriteRenderer = selectionArea.GetComponent<SpriteRenderer>();
        
        // 使用MaterialPropertyBlock来高效修改材质属性
        propertyBlock = new MaterialPropertyBlock();
        selectionSpriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", selectionColor);
        propertyBlock.SetColor("_BorderColor", borderColor);
        propertyBlock.SetFloat("_BorderWidth", borderWidth);
        selectionSpriteRenderer.SetPropertyBlock(propertyBlock);

    }

    void Update()
    {
        HandleSelectionInput();
    }

    private void HandleSelectionInput()
    {
        // 鼠标左键按下 - 开始选择
        if (Input.GetMouseButtonDown(0))
        {
            StartSelection();
        }

        // 鼠标左键按住 - 更新选择区域
        if (Input.GetMouseButton(0))
        {
            UpdateSelectionArea();
        }

        // 鼠标左键释放 - 完成选择
        if (Input.GetMouseButtonUp(0))
        {
            CompleteSelection();
        }

        // 鼠标右键 - 命令选中的单位
        if (Input.GetMouseButtonDown(1))
        {
            IssueCommands();
        }
    }

    private void StartSelection()
    {
        selectionStartPosition = Util.GetMouseWorldPosition2D();
        
        // 清除之前的选择
        foreach (var unit in selectedUnits)
        {
            unit.GetComponent<SelectableUnit>().Deselect();
        }
        selectedUnits.Clear();
        
        // 激活选择区域
        selectionArea.gameObject.SetActive(true);
        selectionArea.localScale = Vector3.zero;
        selectionArea.position = selectionStartPosition;
    }

    private void UpdateSelectionArea()
    {
        Vector3 currentMousePosition = Util.GetMouseWorldPosition2D();
        
        // 计算选择区域的中心点
        Vector3 center = (currentMousePosition + selectionStartPosition) / 2f;
        selectionArea.position = center;
        
        // 计算选择区域的大小
        Vector3 size = new Vector3(
            Mathf.Abs(currentMousePosition.x - selectionStartPosition.x),
            Mathf.Abs(currentMousePosition.y - selectionStartPosition.y),
            1f
        );
        
        // 获取Sprite原始大小并计算缩放比例
        if (selectionSpriteRenderer.sprite != null)
        {
            Vector2 spriteSize = selectionSpriteRenderer.sprite.bounds.size;
            Vector3 scale = new Vector3(
                size.x / spriteSize.x,
                size.y / spriteSize.y,
                1f
            );
            selectionArea.localScale = scale;
        }

        // 计算选择区域的包围盒
        Vector2 min = Vector2.Min(selectionStartPosition, currentMousePosition);
        Vector2 max = Vector2.Max(selectionStartPosition, currentMousePosition);

        Collider2D[] colliders = Physics2D.OverlapAreaAll(selectionStartPosition, currentMousePosition);
        HashSet<SelectableUnit> currentUnits = new HashSet<SelectableUnit>();

        foreach (var collider in colliders)
        {
            var unit = collider.GetComponent<SelectableUnit>();
            if (unit != null)
            {
                currentUnits.Add(unit);
                if (!preselectedUnits.Contains(unit))
                    unit.Preselect();
            }
        }

        // 离开预选区的单位
        foreach (var unit in preselectedUnits)
        {
            if (!currentUnits.Contains(unit))
                unit.Unpreselect();
        }

        // 更新缓存
        preselectedUnits = currentUnits;
    }

    private void CompleteSelection()
    {
        // 获取选择区域内的所有碰撞体
        Vector3 currentMousePosition = Util.GetMouseWorldPosition2D();
        Collider2D[] colliders = Physics2D.OverlapAreaAll(
            selectionStartPosition, 
            currentMousePosition
        );
        
        // 选择符合条件的单位
        foreach (var collider in colliders)
        {

            SelectableUnit selectable = collider.GetComponent<SelectableUnit>();
            if (selectable != null)
            {
                selectable.Select();
                selectedUnits.Add(collider.gameObject);
            }
        }
        
        // 隐藏选择区域
        selectionArea.gameObject.SetActive(false);
    }

    private void IssueCommands()
    {
        Vector3 targetPosition = Util.GetMouseWorldPosition2D();
        
        foreach (var unit in selectedUnits)
        {
          
        }
    }
}
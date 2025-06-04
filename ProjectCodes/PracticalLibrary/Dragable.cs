/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-20
 * Description: 可拖动的游戏对象
 *  
 更新记录：
 2025-06-04 12:28 添加了OnDragStart、OnDrag、OnDragEnd事件
 ****************************************************************************/
using UnityEngine;

using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
public class Dragable : MonoBehaviour
{
    private Vector3 _offset;

    private bool _isDragging = false;


    /// <summary>
    /// 拖动对象的根节点
    /// </summary>
    public Transform root;


    /// <summary>
    /// 阻止拖动的UI名称,也就是说，当这个名字的ui在拖动对象的上方时，拖动对象无法被拖动
    /// </summary>
    public string[] blockUINames;

    /// <summary>
    /// 当这个对象浮在Dragable的对象上时，对象无法被拖动
    /// </summary>
    private GraphicRaycaster _raycaster;



    public Action OnDragStart;
    public Action OnDrag;
    public Action OnDragEnd;

    private void Start()
    {
        _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }


    /// <summary>
    /// 设置拖动对象的Raycaster
    /// </summary>
    /// <param name="raycaster"></param>
    public void SetRaycaster(GraphicRaycaster raycaster)
    {
        _raycaster = raycaster;
    }

    void OnMouseDown()
    {
        if (IsTouchingBlockCanvas())
        {
            return;
        }

        // 检查鼠标点击是否在允许拖动的区域内
        Collider2D draggableArea = GetComponent<Collider2D>();
        if (draggableArea != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            if (!draggableArea.OverlapPoint(mousePosition))
            {
                return; // 如果不在允许的区域内，直接返回
            }
        }

        _offset = root.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        _isDragging = true;
        OnDragStart?.Invoke();
    }

    void OnMouseDrag()
    {
        if (_isDragging)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            root.position = new Vector3(curPosition.x + _offset.x, curPosition.y + _offset.y, 0);
            OnDrag?.Invoke();
        }
    }

    void OnMouseUp()
    {
        _isDragging = false;
        // hasDragged = true;

        //当位移小于0.2时，不执行逻辑
        // if (Vector3.Distance(startPosition, root.position) > 0.2f)
        // {
        //     GameController.Instance.DragCost(root.position);
        // }
        OnDragEnd?.Invoke();
    }



    private EventSystem _eventSystem;
    private bool IsTouchingBlockCanvas()
    {
        if (_raycaster == null)
        {
            return false;
        }
        PointerEventData pointerEventData = new PointerEventData(_eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        _raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {

            foreach (string blockUIName in blockUINames)
            {
                if (result.gameObject.name.Contains(blockUIName))
                {
                    return true;
                }
            }


        }
        return false;
    }

}

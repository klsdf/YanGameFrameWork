/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-20
 * Description: 可拖动的游戏对象
 *
 ****************************************************************************/
using UnityEngine;

using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class Dragable : MonoBehaviour
{
    private Vector3 _offset;

    private Vector3 _startPosition;
    private bool _isDragging = false;

    public Transform root;
    private void Start()
    {
        raycaster = ShopController.Instance.GetGraphicRaycaster();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }
    void OnMouseDown()
    {
        if (IsTouchedShopUI())
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
        _startPosition = root.position;
    }

    void OnMouseDrag()
    {
        // if (IsTouchedUI())
        // {
        //     return;
        // }
        if (_isDragging)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            root.position = new Vector3(curPosition.x + _offset.x, curPosition.y + _offset.y, 0);
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
    }


    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    private bool IsTouchedShopUI()
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {

            if (result.gameObject.name.Contains("ShopPenal"))
            {
                return true;
            }

        }
        return false;
    }

}

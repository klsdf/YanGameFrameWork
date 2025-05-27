/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-24
 * Description: 可拖动面板
 *
 * 修改记录:
 * 2025-05-27 闫辰祥 把ui拖动改为了世界坐标，避免被父元素所影响
 ****************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace YanGameFrameWork.PracticalLibrary
{

    /// <summary>
    /// 加上这个脚本的UI元素可以被拖动
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class DragablePanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private Vector3 _offsetWorld; // 用于记录世界坐标下的偏移
        private Vector3 _initialWorldPosition; // 记录初始世界坐标

        [Header("是否启用回弹防抖功能")]
        public bool enableSnapBack = false;  // 是否启用回弹功能

        [Header("防手抖距离")]
        public float snapBackThreshold = 100f;  // 回弹阈值

        [Header("拖动结束后是否重置位置")]
        public bool resetPositionAfterDrag = false;  // 是否在拖动结束后重置位置

        // 定义 Unity 事件
        public UnityEvent onDragStart;
        public UnityEvent onDrag;
        public UnityEvent onDragEnd;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // 用世界坐标计算偏移
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint))
            {
                _offsetWorld = _rectTransform.position - worldPoint;
                _initialWorldPosition = _rectTransform.position; // 记录初始世界坐标
            }
            onDragStart?.Invoke();  // 触发拖动开始事件
        }

        public void OnDrag(PointerEventData eventData)
        {
            // 用世界坐标拖动
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint))
            {
                _rectTransform.position = worldPoint + _offsetWorld;
                onDrag?.Invoke();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // 用世界坐标判断回弹和重置
            if (enableSnapBack && Vector3.Distance(_rectTransform.position, _initialWorldPosition) < snapBackThreshold)
            {
                _rectTransform.position = _initialWorldPosition;
            }
            else if (resetPositionAfterDrag)
            {
                _rectTransform.position = _initialWorldPosition;
            }
            onDragEnd?.Invoke();
            // 可以在这里添加其他逻辑，比如限制拖动范围或其他行为
        }
    }


}
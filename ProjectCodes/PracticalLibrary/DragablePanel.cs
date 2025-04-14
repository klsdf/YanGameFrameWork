/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-24
 * Description: 可拖动面板
 *
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
        private Vector2 _offset;
        private Vector2 _initialPosition;  // 记录初始位置

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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
            _offset = _rectTransform.anchoredPosition - localPoint;
            _initialPosition = _rectTransform.anchoredPosition;  // 在按下时记录初始位置
            onDragStart?.Invoke();  // 触发拖动开始事件
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            {
                _rectTransform.anchoredPosition = localPoint + _offset;
                onDrag?.Invoke();  // 触发拖动中事件

            }
        }



        public void OnPointerUp(PointerEventData eventData)
        {
            // 如果启用了回弹功能并且移动距离小于阈值，则重置位置
            if (enableSnapBack && Vector2.Distance(_rectTransform.anchoredPosition, _initialPosition) < snapBackThreshold)
            {
                _rectTransform.anchoredPosition = _initialPosition;
            }
            else if (resetPositionAfterDrag)  // 如果启用了拖动结束后重置位置的功能
            {
                _rectTransform.anchoredPosition = _initialPosition;
            }
            onDragEnd?.Invoke();  // 触发拖动结束事件
            // 可以在这里添加其他逻辑，比如限制拖动范围或其他行为
        }
    }


}
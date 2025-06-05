/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-24
 * Description: 可拖动面板
 * 修改记录:
 * 2025-05-27 闫辰祥 把ui拖动改为了世界坐标，避免被父元素所影响
 * 2025-06-05 闫辰祥 添加了右键取消拖动功能,增加了onThresholdDragEnd事件
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
        private Vector3 _offsetWorld; // 用于记录世界坐标下的偏移
        private Vector3 _initialWorldPosition; // 记录初始世界坐标
        public bool isDragging = false;

        [Header("是否启用回弹防抖功能")]
        public bool enableSnapBack = false;  // 是否启用回弹功能

        [Header("防手抖距离")]
        public float snapBackThreshold = 100f;  // 回弹阈值

        [Header("拖动结束后是否重置位置")]
        public bool resetPositionAfterDrag = false;  // 是否在拖动结束后重置位置


        [Header("是否启用右键取消拖动")]
        public bool isRightClickCancelDrag = false;

        // 定义 Unity 事件
        public UnityEvent onDragStart;
        public UnityEvent onDrag;
        public UnityEvent onDragEnd;

        public UnityEvent onThresholdDragEnd;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            // 用世界坐标计算偏移
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint))
            {
                _offsetWorld = _rectTransform.position - worldPoint;
                _initialWorldPosition = _rectTransform.position; // 记录初始世界坐标
            }
            onDragStart?.Invoke();  // 触发拖动开始事件
            isDragging = true;
        }

        private void Update()
        {
            if (isRightClickCancelDrag && Input.GetMouseButtonDown(1) && isDragging)
            {
                print("取消");
                _rectTransform.position = _initialWorldPosition;
                isDragging = false;
            }
        }


        public void OnDrag(PointerEventData eventData)
        {

            if (!isDragging)
                return;
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
            if (!isDragging)
                return;
            // 用世界坐标判断回弹和重置
            if (enableSnapBack)
            {

                if (Vector3.Distance(_rectTransform.position, _initialWorldPosition) < snapBackThreshold)
                {
                    _rectTransform.position = _initialWorldPosition;
                }
                else
                {
                    onThresholdDragEnd?.Invoke();
                }
            }


            // 如果拖动结束，则重置位置
            if (resetPositionAfterDrag)
            {
                _rectTransform.position = _initialWorldPosition;
            }
            onDragEnd?.Invoke();
            isDragging = false;
            // 可以在这里添加其他逻辑，比如限制拖动范围或其他行为
        }
    }


}
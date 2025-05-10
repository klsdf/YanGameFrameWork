/****************************************************************************
 * Author: 闫辰祥
 * Date: 2024-12-25
 * Description: 跟随鼠标移动的UI，最早是用于制作《我思故我在》的眼睛效果
 *
 ****************************************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace YanGameFrameWork.PracticalLibrary
{
    public class UIFollowMouse : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("UI 元素相对于鼠标的最大移动距离。")]
        private float maxDistance = 20f;

        private RectTransform rectTransform;
        private Vector2 originalPosition;

        void Start()
        {
            // 获取 RectTransform 组件
            rectTransform = GetComponent<RectTransform>();

            // 记录初始位置
            originalPosition = rectTransform.anchoredPosition;
        }

        void Update()
        {
            // 获取鼠标在屏幕上的位置
            Vector2 mousePosition = Input.mousePosition;

            // 将鼠标位置从屏幕空间转换为本地空间
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                mousePosition,
                null,
                out Vector2 localMousePosition
            );

            // 计算从初始位置到鼠标位置的偏移量
            Vector2 offset = localMousePosition - originalPosition;

            // 限制偏移量的最大长度
            if (offset.magnitude > maxDistance)
            {
                offset = offset.normalized * maxDistance;
            }

            // 更新 UI 元素的位置
            rectTransform.anchoredPosition = originalPosition + offset;
        }
    }
}

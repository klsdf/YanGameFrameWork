/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-03
 * Description: 补间动画控制器，用于对各种元素进行tween的控制
 *
 ****************************************************************************/
using System;
using System.Linq.Expressions;
using UnityEngine;
using YanGameFrameWork.Singleton;

namespace YanGameFrameWork.TweenSystem
{

    /// <summary>
    /// 补间动画控制器，用于对各种元素进行tween的控制
    /// </summary>
    public class TweenController : Singleton<TweenController>
    {
        /// <summary>
        /// 创建并启动一个Tween，支持链式调用。
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="target">目标对象</param>
        /// <param name="propertySelector">属性选择器</param>
        /// <param name="endValue">结束值</param>
        /// <param name="duration">持续时间</param>
        /// <param name="onComplete">完成回调</param>
        /// <returns>返回TweenController以支持链式调用</returns>
        public TweenController Tween<T, TValue>(
            T target,
            Expression<Func<T, TValue>> propertySelector,
            TValue endValue,
            float duration,
            Action onComplete = null)
        {
            var lerpFunc = TweenLerpSelector<TValue>.LerpFunc;
            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null || !(memberExpression.Member is System.Reflection.PropertyInfo))
            {
                throw new ArgumentException("属性选择器必须是一个属性访问表达式。");
            }

            var propertyInfo = (System.Reflection.PropertyInfo)memberExpression.Member;
            TValue startValue = (TValue)propertyInfo.GetValue(target);
            Action<float> onUpdate = t =>
            {
                TValue value = lerpFunc(startValue, endValue, t);
                propertyInfo.SetValue(target, value);
            };
            Tween tween = new Tween(duration, 0f, 1f, onUpdate, onComplete);
            tween.Start();
            return this;
        }

        /// <summary>
        /// 创建一个Tween
        /// </summary>
        /// <param name="startValue">开始值</param>
        /// <param name="endValue">结束值</param>
        /// <param name="duration">持续时间</param>
        /// <param name="onUpdate">更新回调</param>
        /// <param name="onComplete">完成回调</param>
        /// <returns></returns>
        /// 别忘了创建完start一下
        public Tween CreateTween(float startValue, float endValue, float duration, Action<float> onUpdate, Action onComplete = null)
        {
            Tween tween = new Tween(duration, startValue, endValue, onUpdate, onComplete);
            return tween;
        }

        public Tween CreateAndStartTween(float startValue, float endValue, float duration, Action<float> onUpdate, Action onComplete = null)
        {
            Tween tween = CreateTween(startValue, endValue, duration, onUpdate, onComplete);
            tween.Start();
            return tween;
        }

        public void StartTween(Tween tween)
        {
            StartCoroutine(tween.Tweening(tween.startValue, tween.endValue, tween.duration, tween.onUpdate, tween.onComplete));
        }



        /// <summary>
        /// 移动UI到Canvas之外
        /// </summary>
        /// <param name="rectTransform">UI的RectTransform</param>
        /// <param name="canvasRectTransform">Canvas的RectTransform</param>
        /// <param name="speed">移动速度</param>
        /// <param name="onComplete">完成回调</param>
        /// <returns>返回TweenController以支持链式调用</returns>
        public TweenController MoveUIOutOfCanvas(RectTransform rectTransform, RectTransform canvasRectTransform, float speed, Action onComplete = null)
        {
            Vector2 startPosition = rectTransform.anchoredPosition;

            Vector2 halfSize = rectTransform.rect.size * 0.5f;
            Vector2 canvasSize = canvasRectTransform.rect.size;
            Vector2 endPosition = new Vector2(startPosition.x, canvasSize.y + halfSize.y);

            float distance = Vector2.Distance(startPosition, endPosition);
            float duration = distance / speed;

            Action<float> onUpdate = t =>
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
            };

            Tween tween = new Tween(duration, 0f, 1f, onUpdate, onComplete);
            tween.Start();
            return this;
        }



    }

    public static class TweenLerpSelector<T>
    {
        public static Func<T, T, float, T> LerpFunc
        {
            get
            {
                if (typeof(T) == typeof(float))
                    return (a, b, t) => (T)(object)Mathf.Lerp((float)(object)a, (float)(object)b, t);
                if (typeof(T) == typeof(Color))
                    return (a, b, t) => (T)(object)Color.Lerp((Color)(object)a, (Color)(object)b, t);
                // 继续扩展其他类型
                throw new NotSupportedException($"不支持类型 {typeof(T)} 的Lerp");
            }
        }
    }
}

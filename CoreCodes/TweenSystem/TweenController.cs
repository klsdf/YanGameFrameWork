using System;
using YanGameFrameWork.Singleton;


namespace YanGameFrameWork.TweenSystem
{
    public class TweenController : Singleton<TweenController>
    {

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

    }
}

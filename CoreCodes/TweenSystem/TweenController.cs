using System;
using YanGameFrameWork.Singleton;

public class TweenController : Singleton<TweenController>
{

    public Tween CreateTween(float startValue, float endValue, float duration, Action<float> onUpdate, Action onComplete = null)
    {
        Tween tween = new Tween(duration, startValue, endValue, onUpdate, onComplete);
        return tween;
    }

    public void StartTween(Tween tween)
    {
        StartCoroutine(tween.Tweening(tween.startValue, tween.endValue, tween.duration, tween.onUpdate, tween.onComplete));
    }

}
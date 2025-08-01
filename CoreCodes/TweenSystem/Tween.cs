using System;
using System.Collections;
using UnityEngine;

namespace YanGameFrameWork.TweenSystem
{
    public class Tween
    {
        public float duration;
        public float startValue;
        public float endValue;

        public Action onStart;
        public Action<float> onUpdate;
        public Action onComplete;

        public Tween(float duration, float startValue, float endValue, Action<float> onUpdate, Action onComplete)
        {
            this.duration = duration;
            this.startValue = startValue;
            this.endValue = endValue;
            this.onUpdate = onUpdate;
            this.onComplete = onComplete;
        }



        public IEnumerator Tweening(float startValue, float endValue, float duration, Action<float> onUpdate, Action onComplete = null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float currentValue = Mathf.Lerp(startValue, endValue, t);
                onUpdate?.Invoke(currentValue);
                yield return null;
            }

            // Ensure the final value is set
            onUpdate?.Invoke(endValue);
            onComplete?.Invoke();
        }

    }
}

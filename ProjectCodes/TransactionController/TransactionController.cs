using UnityEngine;
using UnityEngine.UI;
using YanGameFrameWork.Singleton;   
using Sirenix.OdinInspector;
using System.Collections;
using System;


public class TransactionController : Singleton<TransactionController>
{
    // 通过Inspector拖拽赋值，或在代码中赋值
    public Image maskImage;
    
    private Material maskMaterial;

    // 动画时长（可调整）
    public float animDuration = 1f;

    private void Start()
    {
        maskMaterial = maskImage.material;
        maskMaterial.SetFloat("_MaskThreshold", 0f);
    }

    /// <summary>
    /// 触发遮罩阈值动画
    /// </summary>
    [Button("触发遮罩阈值动画")]
    public void PlayMaskThresholdAnim(Action onMaskComplete,Action onMaskDisappear)
    {
        StopAllCoroutines();
        StartCoroutine(MaskThresholdAnimCoroutine(onMaskComplete,onMaskDisappear));
    }

    private IEnumerator MaskThresholdAnimCoroutine(Action onMaskComplete,Action onMaskDisappear)
    {
        float t = 0f;
        float start = 0f;
        float end = 0.2f;

         yield return new WaitForSecondsRealtime(0.1f);

        // 0 -> 0.3
        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            float value = Mathf.Lerp(start, end, t / animDuration);
            SetMaskThreshold(value);
            yield return null;
        }
        SetMaskThreshold(end);
        onMaskComplete?.Invoke();

        // 停止0.4秒 - 使用 WaitForSecondsRealtime 确保在时间暂停时也能工作
        yield return new WaitForSecondsRealtime(0.4f);

        // 0.3 -> 0
        t = 0f;
        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            float value = Mathf.Lerp(end, start, t / animDuration);
            SetMaskThreshold(value);
            yield return null;
        }
        SetMaskThreshold(start);
        onMaskDisappear?.Invoke();
    }

    public void SetMaskThreshold(float value)
    {
        if (maskMaterial != null)
        {
            maskMaterial.SetFloat("_MaskThreshold", value);
        }
        else
        {
            Debug.LogWarning("maskMaterial未赋值！");
        }
    }
}
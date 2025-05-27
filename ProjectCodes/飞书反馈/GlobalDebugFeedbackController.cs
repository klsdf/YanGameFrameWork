using UnityEngine;
using System.Collections;
using System;
using TMPro;
public class GlobalDebugFeedbackController : MonoBehaviour
{
    [Header("常驻文本")]
    public TMP_Text permanentText;
    [Header("反馈面板")]
    public GlobalDebugFeedbackPanel feedbackPanel;
    [Header("触发反馈面板的按键")]
    public KeyCode feedbackKey = KeyCode.F1;

    void Start()
    {
        feedbackPanel.gameObject.SetActive(false);
        SetNormalText();
    }

    public void SetNormalText()
    {
        permanentText.text = YanGF.Localization.GetText("如果发现bug,请按下F1键反馈");
    }

    public void SetFeedbackingText()
    {
        permanentText.text = YanGF.Localization.GetText("按f1可以随时返回游戏喵~");
    }

    void Update()
    {
        if (Input.GetKeyDown(feedbackKey))
        {


            if (feedbackPanel.gameObject.activeSelf == false)
            {
                SetFeedbackingText();
                Time.timeScale = 0;
                CaptureAndShow((sprite, tex) =>
                {
                    feedbackPanel.screenshot.sprite = sprite;
                    feedbackPanel.gameObject.SetActive(true);
                    feedbackPanel.screenshotTexture = tex;
                });

            }
            else
            {
                feedbackPanel.gameObject.SetActive(false);
                Time.timeScale = 1;
                SetNormalText();
            }
        }
    }

    /// <summary>
    /// 截图并显示
    /// </summary>
    /// <param name="onCaptured"></param>
    public void CaptureAndShow(Action<Sprite, Texture2D> onCaptured)
    {
        StartCoroutine(CaptureAndReturnSprite(onCaptured));
    }

    /// <summary>
    /// 截图并返回Sprite和Texture2D
    /// </summary>
    /// <param name="onCaptured"></param>
    private IEnumerator CaptureAndReturnSprite(System.Action<Sprite, Texture2D> onCaptured)
    {
        // 等待渲染结束
        yield return new WaitForEndOfFrame();

        // 截图
        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
        // 创建Sprite
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        onCaptured?.Invoke(sp, tex);
    }

}

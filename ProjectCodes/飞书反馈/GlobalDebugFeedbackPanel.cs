using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using YanGameFrameWork.LarkAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class GlobalDebugFeedbackPanel : MonoBehaviour
{
    public Image screenshot;


    [NonSerialized]
    public Texture2D screenshotTexture;
    public TMP_InputField userInputField;
    public Button send;
    public TMP_Text feedbackText;
    public Button closeBtn;

    public GlobalDebugFeedbackController _controller;


    void Start()
    {
        send.onClick.AddListener(Send);
        closeBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
            _controller.SetNormalText();
        });
    }


    public void Init(Sprite sprite)
    {
        screenshot.sprite = sprite;
    }


    void OnEnable()
    {
        feedbackText.enabled = false;
    }


    public void Send()
    {
        // 1. 生成临时文件路径
        string tempPath = Application.temporaryCachePath + "/temp_screenshot.png";
        // 2. 将截图写入临时文件
        File.WriteAllBytes(tempPath, screenshotTexture.EncodeToPNG());
        // 3. 上传临时文件
        string fileToken = LarkRequester.UploadImageAndGetFileToken(tempPath);
        // 4. 删除临时文件
        File.Delete(tempPath);

        // 5. 其余逻辑不变
        var recordBody = new RecordBody(new Fields(
            记录ID: LarkRequester.GetAllRecordsCount() + 1,
            反馈内容: userInputField.text,
            创建时间: DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            截图: new List<Attachment> { new Attachment(fileToken) }
        ));
        string result = LarkRequester.AddRecord(recordBody);
        Debug.Log(result);
        feedbackText.text = YanGF.Localization.Translate("反馈成功！我们会尽快处理！");
        feedbackText.enabled = true;
    }


}

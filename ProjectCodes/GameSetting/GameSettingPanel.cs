using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YanGameFrameWork.UISystem;

public class GameSettingPanel : UIPanelBase
{

    [Header("语言下拉框")]
    public TMP_Dropdown dropdown;

    [Header("主音量滑块")]
    public Slider masterVolumeSlider;
    [Header("音乐音量滑块")]
    public Slider musicVolumeSlider;
    [Header("音效音量滑块")]
    public Slider effectsVolumeSlider;

    [Header("分辨率下拉框")]
    public TMP_Dropdown resolutionDropdown;


    [Header("全屏按钮")]
    public Button fullScreenButton;

    [Header("锁帧")]
    public Button lockFrameButton;

    [Header("退出按钮")]
    public Button exitButton;

    void Start()
    {
        // 设置语言下拉框的选项
        dropdown.options.Clear();
        dropdown.options.Add(new TMP_Dropdown.OptionData("中文"));
        dropdown.options.Add(new TMP_Dropdown.OptionData("English"));
        dropdown.options.Add(new TMP_Dropdown.OptionData("日本語"));

        // 添加语言下拉框监听器
        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });

        // 添加音量滑块监听器
        masterVolumeSlider.onValueChanged.AddListener(delegate { SetVolume("master", masterVolumeSlider.value); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { SetVolume("music", musicVolumeSlider.value); });
        effectsVolumeSlider.onValueChanged.AddListener(delegate { SetVolume("effects", effectsVolumeSlider.value); });

        // 设置分辨率下拉框的选项
        resolutionDropdown.options.Clear();
        Resolution[] resolutions = Screen.resolutions;
        foreach (Resolution res in resolutions)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(res.width + "x" + res.height));
        }

        // 添加分辨率下拉框监听器
        resolutionDropdown.onValueChanged.AddListener(delegate { ResolutionItemSelected(resolutionDropdown); });

        // 添加按钮监听器
        fullScreenButton.onClick.AddListener(ToggleFullScreen);
        lockFrameButton.onClick.AddListener(ToggleFrameLock);
        exitButton.onClick.AddListener(ExitApplication);
    }

    void DropdownItemSelected(TMP_Dropdown dropdown)
    {
        int index = dropdown.value;
        Debug.Log("选择了: " + dropdown.options[index].text);
    }

    void SetVolume(string volumeType, float value)
    {
        // 使用对数刻度将滑块值映射到 -80 到 20 的范围
        float minDb = -80f;
        float maxDb = 20f;
        float mappedValue = Mathf.Log10(value * 9 + 1) * (maxDb - minDb) + minDb;
        Debug.Log(volumeType + " 当前值: " + mappedValue);

        switch (volumeType)
        {
            case "master":
                YanGF.Audio.SetMasterVolume(mappedValue);
                break;
            case "music":
                YanGF.Audio.SetMusicVolume(mappedValue);
                break;
            case "effects":
                YanGF.Audio.SetEffectsVolume(mappedValue);
                break;
        }
    }

    void ResolutionItemSelected(TMP_Dropdown dropdown)
    {
        int index = dropdown.value;
        Resolution selectedResolution = Screen.resolutions[index];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        Debug.Log("分辨率设置为: " + selectedResolution.width + "x" + selectedResolution.height);
    }

    void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Debug.Log("全屏模式: " + (Screen.fullScreen ? "开启" : "关闭"));
    }

    void ToggleFrameLock()
    {
        // 假设锁定帧率为60
        Application.targetFrameRate = Application.targetFrameRate == 60 ? -1 : 60;
        Debug.Log("锁帧: " + (Application.targetFrameRate == 60 ? "开启" : "关闭"));
    }

    void ExitApplication()
    {
        Debug.Log("退出应用程序");
        Application.Quit();
    }
}

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



    public Button closeButton;

    GameSettingData _gameSettingData;


    const string _saveFileName = "GameSettingData";

    void Start()
    {
        InitializeLanguageDropdown();
        InitializeVolumeSliders();
        InitializeResolutionDropdown();
        InitializeButtons();
        closeButton.onClick.AddListener(() =>
        {
            YanGF.UI.PopPanel();
        });
    }

    void InitializeLanguageDropdown()
    {
        // 设置语言下拉框的选项
        dropdown.options.Clear();
        dropdown.options.Add(new TMP_Dropdown.OptionData("中文"));
        dropdown.options.Add(new TMP_Dropdown.OptionData("English"));
        dropdown.options.Add(new TMP_Dropdown.OptionData("日本語"));

        // 添加语言下拉框监听器
        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

    void InitializeVolumeSliders()
    {
        // 添加音量滑块监听器
        masterVolumeSlider.onValueChanged.AddListener(delegate { SetVolume("master", masterVolumeSlider.value); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { SetVolume("music", musicVolumeSlider.value); });
        effectsVolumeSlider.onValueChanged.AddListener(delegate { SetVolume("effects", effectsVolumeSlider.value); });
    }

    void InitializeResolutionDropdown()
    {
        // 设置分辨率下拉框的选项
        resolutionDropdown.options.Clear();
        Resolution[] resolutions = Screen.resolutions;
        foreach (Resolution res in resolutions)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(res.width + "x" + res.height));
        }

        // 添加分辨率下拉框监听器
        resolutionDropdown.onValueChanged.AddListener(delegate { ResolutionItemSelected(resolutionDropdown); });
    }

    void InitializeButtons()
    {
        // 添加按钮监听器
        fullScreenButton.onClick.AddListener(ToggleFullScreen);
        lockFrameButton.onClick.AddListener(ToggleFrameLock);
        exitButton.onClick.AddListener(ExitApplication);
    }

    void DropdownItemSelected(TMP_Dropdown dropdown)
    {
        int index = dropdown.value;
        Debug.Log("选择了: " + dropdown.options[index].text);
        _gameSettingData.languageIndex = index;
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
                _gameSettingData.masterVolume = value;
                break;
            case "music":
                YanGF.Audio.SetMusicVolume(mappedValue);
                _gameSettingData.musicVolume = value;
                break;
            case "effects":
                YanGF.Audio.SetEffectsVolume(mappedValue);
                _gameSettingData.effectsVolume = value;
                break;
        }
    }

    void ResolutionItemSelected(TMP_Dropdown dropdown)
    {
        int index = dropdown.value;
        Resolution selectedResolution = Screen.resolutions[index];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        Debug.Log("分辨率设置为: " + selectedResolution.width + "x" + selectedResolution.height);
        _gameSettingData.resolutionIndex = index;
    }

    void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        _gameSettingData.isFullScreen = Screen.fullScreen;
        Debug.Log("全屏模式: " + (Screen.fullScreen ? "开启" : "关闭"));
    }

    void ToggleFrameLock()
    {
        // 假设锁定帧率为60
        Application.targetFrameRate = Application.targetFrameRate == 60 ? -1 : 60;
        _gameSettingData.isFrameLocked = Application.targetFrameRate == 60;
        Debug.Log("锁帧: " + (Application.targetFrameRate == 60 ? "开启" : "关闭"));
    }

    void ExitApplication()
    {
        Debug.Log("退出应用程序");
        Application.Quit();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _gameSettingData = YanGF.Save.Load("GameSettingData", new GameSettingData(), _saveFileName);

        // 设置滑动条的值
        masterVolumeSlider.value = _gameSettingData.masterVolume;
        musicVolumeSlider.value = _gameSettingData.musicVolume;
        effectsVolumeSlider.value = _gameSettingData.effectsVolume;

        // 设置下拉框的值
        dropdown.value = _gameSettingData.languageIndex;
        dropdown.RefreshShownValue();

        resolutionDropdown.value = _gameSettingData.resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // 设置全屏和锁帧状态
        Screen.fullScreen = _gameSettingData.isFullScreen;
        Application.targetFrameRate = _gameSettingData.isFrameLocked ? 60 : -1;
    }



    public override void OnExit()
    {
        base.OnExit();
        if (_gameSettingData != null)
        {
            YanGF.Save.Save("GameSettingData", _gameSettingData, _saveFileName);
        }
    }

}

/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-25
 * Description: 游戏设置面板的基本功能
 *
 ****************************************************************************/


using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YanGameFrameWork.UISystem;




namespace YanGameFrameWork.GameSetting
{
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
        public YanSingleToggle fullScreenButton;

        [Header("全屏文本")]
        public TMP_Text fullScreenText;

        [Header("锁帧")]
        public YanSingleToggle lockFrameButton;

        [Header("锁帧文本")]
        public TMP_Text lockFrameText;

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


        /// <summary>
        /// 初始化语言下拉框
        /// </summary>
        void InitializeLanguageDropdown()
        {
            // 设置语言下拉框的选项
            dropdown.options.Clear();
            dropdown.options.Add(new TMP_Dropdown.OptionData("中文"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("English"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("日本語"));

            // 添加语言下拉框监听器
            dropdown.onValueChanged.AddListener(delegate
            { DropdownItemSelected(dropdown); });
        }

        /// <summary>
        /// 初始化音量滑块
        /// </summary>
        void InitializeVolumeSliders()
        {
            // 添加音量滑块监听器
            masterVolumeSlider.onValueChanged.AddListener(delegate
            { SetVolume("master", masterVolumeSlider.value); });
            musicVolumeSlider.onValueChanged.AddListener(delegate
            { SetVolume("music", musicVolumeSlider.value); });
            effectsVolumeSlider.onValueChanged.AddListener(delegate
            { SetVolume("effects", effectsVolumeSlider.value); });
        }

        /// <summary>
        /// 初始化分辨率下拉框
        /// </summary>
        void InitializeResolutionDropdown()
        {
            // 清空分辨率下拉框的选项
            resolutionDropdown.options.Clear();

            // 必备支持分辨率
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1920x1080"));
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1366x768"));
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("1280x720"));

            // 推荐支持分辨率
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData("2560x1440"));

            // 添加分辨率下拉框监听器
            resolutionDropdown.onValueChanged.AddListener(delegate
            { ResolutionItemSelected(resolutionDropdown); });
        }

        /// <summary>
        /// 初始化各个按钮
        /// </summary>
        void InitializeButtons()
        {
            // 添加按钮监听器
            fullScreenButton.SetOnToggleEvent(ToggleFullScreen);
            lockFrameButton.SetOnToggleEvent(ToggleFrameLock);
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

        /// <summary>
        /// 分辨率下拉框选项被选中
        /// </summary>
        void ResolutionItemSelected(TMP_Dropdown dropdown)
        {
            int index = dropdown.value;
            Resolution selectedResolution = Screen.resolutions[index];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
            Debug.Log("分辨率设置为: " + selectedResolution.width + "x" + selectedResolution.height);
            _gameSettingData.resolutionIndex = index;
        }

        /// <summary>
        /// 切换全屏模式
        /// </summary>
        void ToggleFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
            _gameSettingData.isFullScreen = isFullScreen;
            fullScreenText.text = isFullScreen ? "开启" : "关闭";
            Debug.Log("全屏模式: " + (Screen.fullScreen ? "开启" : "关闭"));
        }

        /// <summary>
        /// 切换锁帧模式
        /// </summary>
        void ToggleFrameLock(bool isFrameLocked)
        {
            // 假设锁定帧率为60
            Application.targetFrameRate = isFrameLocked ? 60 : -1;
            _gameSettingData.isFrameLocked = isFrameLocked;
            lockFrameText.text = isFrameLocked ? "开启" : "关闭";
            Debug.Log("锁帧: " + (isFrameLocked ? "开启" : "关闭"));
        }

        /// <summary>
        /// 退出应用程序
        /// </summary>
        void ExitApplication()
        {
            Debug.Log("退出应用程序");
            Application.Quit();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _gameSettingData = YanGF.Save.Load("GameSettingData", new GameSettingData(), _saveFileName);

            // 设置音量
            masterVolumeSlider.value = _gameSettingData.masterVolume;
            SetVolume("master", _gameSettingData.masterVolume);
            musicVolumeSlider.value = _gameSettingData.musicVolume;
            SetVolume("music", _gameSettingData.musicVolume);
            effectsVolumeSlider.value = _gameSettingData.effectsVolume;
            SetVolume("effects", _gameSettingData.effectsVolume);


            // 设置分辨率
            resolutionDropdown.value = _gameSettingData.resolutionIndex;
            resolutionDropdown.RefreshShownValue();

            // 设置语言
            dropdown.value = _gameSettingData.languageIndex;
            dropdown.RefreshShownValue();

            // 设置全屏和锁帧状态
            if (_gameSettingData.isFullScreen)
            {
                fullScreenText.text = "开启";
                fullScreenButton.TurnOn();
            }
            else
            {
                fullScreenText.text = "关闭";
                fullScreenButton.TurnOff();
            }


            if (_gameSettingData.isFrameLocked)
            {
                lockFrameText.text = "开启";
                lockFrameButton.TurnOn();
            }
            else
            {
                lockFrameText.text = "关闭";
                lockFrameButton.TurnOff();
            }
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
}

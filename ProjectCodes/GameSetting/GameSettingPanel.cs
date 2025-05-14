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
        public TMP_Text fullScreenStateText;

        [Header("锁帧")]
        public YanSingleToggle lockFrameButton;

        [Header("锁帧文本")]
        public TMP_Text lockFrameStateText;

        [Header("退出按钮")]
        public Button exitButton;


        [Header("关闭按钮")]
        public Button closeButton;

        GameSettingData _gameSettingData;





        [SerializeField]
        [Header("自定义分辨率")]
        public Vector2Int[] customResolutions = new Vector2Int[]
        {
            new Vector2Int(1280, 720),
            new Vector2Int(1920, 1080),
            new Vector2Int(2560, 1440)
        };


        [Header("各种按钮的文本")]
        public TMP_Text masterVolumeText;
        public TMP_Text musicVolumeText;
        public TMP_Text effectsVolumeText;
        public TMP_Text resolutionText;
        public TMP_Text fullScreenText;
        public TMP_Text languageText;
        public TMP_Text lockFrameText;
        public TMP_Text exitText;





        const string _saveFileName = "GameSettingData";

        public override void ChildStart()
        {
            base.ChildStart();
            InitializeLanguageDropdown();
            InitializeVolumeSliders();
            InitializeResolutionDropdown();
            InitializeButtons();
            closeButton.onClick.AddListener(() =>
            {
                YanGF.UI.PopPanel();
            });
        }

        public override void OnLocalize()
        {
            masterVolumeText.text = YanGF.Localization.Translate("主音量");
            musicVolumeText.text = YanGF.Localization.Translate("音乐音量");
            effectsVolumeText.text = YanGF.Localization.Translate("音效音量");
            resolutionText.text = YanGF.Localization.Translate("分辨率");
            fullScreenText.text = YanGF.Localization.Translate("全屏");
            languageText.text = YanGF.Localization.Translate("语言");
            lockFrameText.text = YanGF.Localization.Translate("锁60帧");
            exitText.text = YanGF.Localization.Translate("退出游戏");
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

            // 使用customResolutions数组初始化分辨率选项
            foreach (var resolution in customResolutions)
            {
                string optionText = $"{resolution.x}x{resolution.y}";
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(optionText));
            }

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

            switch (dropdown.options[index].text)
            {
                case "中文":
                    YanGF.Localization.SwitchToChinese();
                    break;
                case "English":
                    YanGF.Localization.SwitchToEnglish();
                    break;
                case "日本語":
                    YanGF.Localization.SwitchToJapanese();
                    break;

            }

            _gameSettingData.languageIndex = index;
        }

        void SetVolume(string volumeType, float value)
        {
            // 使用对数刻度将滑块值映射到 -80 到 20 的范围
            float minDb = -80f;
            float maxDb = 20f;
            float mappedValue = Mathf.Log10(value * 9 + 1) * (maxDb - minDb) + minDb;
            // Debug.Log(volumeType + " 当前值: " + mappedValue);

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

            // 从customResolutions中获取选中的分辨率
            if (index >= 0 && index < customResolutions.Length)
            {
                Vector2Int selectedResolution = customResolutions[index];
                Screen.SetResolution(selectedResolution.x, selectedResolution.y, Screen.fullScreen);
                Debug.Log("分辨率设置为: " + selectedResolution.x + "x" + selectedResolution.y);
                _gameSettingData.resolutionIndex = index;
            }
            else
            {
                Debug.LogWarning("无效的分辨率索引");
            }
        }

        /// <summary>
        /// 切换全屏模式
        /// </summary>
        void ToggleFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
            _gameSettingData.isFullScreen = isFullScreen;
            fullScreenStateText.text = GetToggleStatText(isFullScreen);
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
            lockFrameStateText.text = GetToggleStatText(isFrameLocked);
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
                fullScreenStateText.text = GetToggleStatText(true);
                fullScreenButton.TurnOn();
            }
            else
            {
                fullScreenStateText.text = GetToggleStatText(false);
                fullScreenButton.TurnOff();
            }


            if (_gameSettingData.isFrameLocked)
            {
                lockFrameStateText.text = GetToggleStatText(true);
                lockFrameButton.TurnOn();
            }
            else
            {
                lockFrameStateText.text = GetToggleStatText(false);
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



        string GetToggleStatText(bool isOn)
        {
            return isOn ? YanGF.Localization.Translate("开启") : YanGF.Localization.Translate("关闭");
        }

        void OnEnable()
        {
            YanGF.Localization.OnLanguageChanged += OnLocalize;
        }

        void OnDisable()
        {
            YanGF.Localization.OnLanguageChanged -= OnLocalize;
        }
    }
}

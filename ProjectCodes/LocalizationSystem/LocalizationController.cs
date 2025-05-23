/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-20
 * Description: 本地化系统
 *
 * 修改记录:
 * 2025-04-26 闫辰祥 增加了tableName，取消使用枚举来指定本地化表的名称，而是使用字符串来指定
 * 2025-05-20 闫辰祥 大规模重构代码，让其变为适配器，支持unity 自带的本地化框架和自定义两种本地化方式
 * 2025-05-21 闫辰祥 增加繁体中文的适配，把语言类型改为enum
 ****************************************************************************/
using System;
using YanGameFrameWork.Singleton;
using YanGameFrameWork.Editor;
using UnityEngine;

namespace YanGameFrameWork.LocalizationSystem
{
    public class LocalizationController : Singleton<LocalizationController>
    {
        public enum LocalizationType
        {
            Unity,
            Custom
        }


        /// <summary>
        /// 本地化适配器
        /// </summary>
        private ILocalizationAdapter _adapter;


        [Header("本地化方案的类型")]
        public LocalizationType localizationType = LocalizationType.Custom;

        [Header("本地化表名")]
        public string tableName;


        [Header("初始化语言")]
        public LanguageType initLanguageType;


        [SerializeField]
        [Header("当前的语言")]
        private LanguageType _currentLanguageType;
        public LanguageType CurrentLanguageType => _currentLanguageType;


        [Header("是否启用本地化")]
        public bool enable = true;


        /// <summary>
        /// 语言切换事件，是给其他人调用的
        /// </summary>
        public event Action OnLanguageChanged;

        protected override void Awake()
        {
            base.Awake();
            InitAdapter();
            ChangeToInitLanguage();
        }



        private void ChangeToInitLanguage()
        {
            switch (initLanguageType)
            {
                case LanguageType.SimplifiedChinese:
                    SwitchToSimplifiedChinese();
                    break;
                case LanguageType.TraditionalChinese:
                    SwitchToTraditionalChinese();
                    break;
                case LanguageType.Japanese:
                    SwitchToJapanese();
                    break;
                case LanguageType.English:
                    SwitchToEnglish();
                    break;
            }
        }


        /// <summary>
        /// 初始化适配器
        /// </summary>
        private void InitAdapter()
        {
            if (localizationType == LocalizationType.Unity)
            {
                _adapter = new UnityLocalizationAdapter(tableName);
            }
            else
            {
                _adapter = new CustomLocalizationAdapter();
            }
        }



        protected override void OnDestroy()
        {
            _adapter.OnDestroy();
            base.OnDestroy();
        }




        ////////////////////////////////////公共方法//////////////////////////////////////

        public string Translate(string key, string chineseText = null)
        {

            if (key == null)
            {
                YanGF.Debug.LogError(nameof(LocalizationController), "key为null！");
                return null;
            }

            // 打印调用者信息（包含行号）
            var stackTrace = new System.Diagnostics.StackTrace(true); // 传true以获取文件和行号
            var frame = stackTrace.GetFrame(1);
            var method = frame.GetMethod();
            Type declaringType = method.DeclaringType;
            int lineNumber = frame.GetFileLineNumber();

            MetaData metaData = new MetaData()
            {
                className = declaringType.FullName,
                methodName = method.Name,
                lineNumber = lineNumber
            };

            if (enable == false)
            {

                //如果传入了中文翻译则使用中文翻译，否则使用key
                return chineseText == null ? key : chineseText;
            }
            return _adapter.Translate(key.Trim(), metaData, chineseText);
        }





        private void OnLocaleChanged()
        {
            OnLanguageChanged?.Invoke();
        }


        ///////////////////////////////////面板上的工具方法/////////////////////////////////////

        [Button("检查未本地化字符串")]
        public void CheckLocalization(string folderPath = "Assets/Scripts/CardDraw")
        {
            print("检查未本地化字符串");
            // LocalizationChecker.CheckLocalization();
            LocalizationRoslynChecker.CheckLocalizationWithRoslyn(folderPath);
        }

        ///////////////////////////////////公用API/////////////////////////////////////


        /// <summary>
        /// 切换到简体中文
        /// </summary>
        public void SwitchToSimplifiedChinese()
        {
            _adapter.SwitchLanguage(LanguageType.SimplifiedChinese);
            _currentLanguageType = LanguageType.SimplifiedChinese;
            OnLocaleChanged();
        }

        /// <summary>
        /// 切换到繁体中文
        /// </summary>
        public void SwitchToTraditionalChinese()
        {
            _adapter.SwitchLanguage(LanguageType.TraditionalChinese);
            _currentLanguageType = LanguageType.TraditionalChinese;
            OnLocaleChanged();
        }

        /// <summary>
        /// 切换到日文
        /// </summary>
        public void SwitchToJapanese()
        {
            _adapter.SwitchLanguage(LanguageType.Japanese);
            _currentLanguageType = LanguageType.Japanese;
            OnLocaleChanged();
        }

        /// <summary>
        /// 切换到英文
        /// </summary>
        public void SwitchToEnglish()
        {
            _adapter.SwitchLanguage(LanguageType.English);
            _currentLanguageType = LanguageType.English;
            OnLocaleChanged();
        }

        public LanguageType GetCurrentLanguage()
        {
            return _adapter.GetCurrentLanguage();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            // 只有在运行时才切换
            SwitchLanguageByType(_currentLanguageType);
        }
#endif

        private void SwitchLanguageByType(LanguageType type)
        {
            switch (type)
            {
                case LanguageType.SimplifiedChinese:
                    SwitchToSimplifiedChinese();
                    break;
                case LanguageType.TraditionalChinese:
                    SwitchToTraditionalChinese();
                    break;
                case LanguageType.Japanese:
                    SwitchToJapanese();
                    break;
                case LanguageType.English:
                    SwitchToEnglish();
                    break;
            }
        }
    }
}
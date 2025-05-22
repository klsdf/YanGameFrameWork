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
using UnityEngine;
using System;
using YanGameFrameWork.Singleton;

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

        public LocalizationType localizationType;

        public string tableName;

        public LanguageType initLanguageType;

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


        ///////////////////////////////////公用API/////////////////////////////////////


        /// <summary>
        /// 切换到简体中文
        /// </summary>
        public void SwitchToSimplifiedChinese()
        {
            _adapter.SwitchLanguage(LanguageType.SimplifiedChinese);
            OnLocaleChanged();
        }

        /// <summary>
        /// 切换到繁体中文
        /// </summary>
        public void SwitchToTraditionalChinese()
        {
            _adapter.SwitchLanguage(LanguageType.TraditionalChinese);
            OnLocaleChanged();
        }

        /// <summary>
        /// 切换到日文
        /// </summary>
        public void SwitchToJapanese()
        {
            _adapter.SwitchLanguage(LanguageType.Japanese);
            OnLocaleChanged();
        }

        /// <summary>
        /// 切换到英文
        /// </summary>
        public void SwitchToEnglish()
        {
            _adapter.SwitchLanguage(LanguageType.English);
            OnLocaleChanged();
        }

        public LanguageType GetCurrentLanguage()
        {
            return _adapter.GetCurrentLanguage();
        }

    }
}
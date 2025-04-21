/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-20
 * Description: 本地化系统
 *
 ****************************************************************************/
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using YanGameFrameWork.CoreCodes;


namespace YanGameFrameWork.LocalizationSystem
{

    public enum TableType
    {
        Card,
    }

    public class LocalizationController : Singleton<LocalizationController>
    {

        public bool enable = true;


        private StringTable _tableStory;
        private StringTable tableStory
        {
            get
            {
                if (_tableStory == null)
                {
                    _tableStory = LocalizationSettings.StringDatabase.GetTable(TableType.Card.ToString());
                }
                return _tableStory;
            }
            set
            {
                _tableStory = value;
            }
        }

        void Start()
        {
            if (LocalizationSettings.AvailableLocales.Locales.Count > 0)
            {
                GetLocalizationTable(TableType.Card);
            }
            else
            {
                LocalizationSettings.InitializationOperation.Completed += OnLocalizationInitialized;
            }
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            SwitchToEnglish();
        }

        void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        /// <summary>
        /// 本地化初始化完成
        /// </summary>
        private void OnLocalizationInitialized(AsyncOperationHandle<LocalizationSettings> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Localization initialized successfully!");
                //这里临时用一下Card
                GetLocalizationTable(TableType.Card);
            }
            else
            {
                Debug.LogError("Localization initialization failed.");
            }
        }

        /// <summary>
        /// 切换语言
        /// </summary>
        private void OnLocaleChanged(Locale newLocale)
        {
            print("切换语言：" + newLocale.Identifier.Code);
            LocalizationSettings.SelectedLocale = newLocale;
            //这里临时用一下Card
            GetLocalizationTable(TableType.Card);
        }




        /// <summary>
        /// 获取本地化表
        /// </summary>
        private void GetLocalizationTable(TableType tableType)
        {
            try
            {
                tableStory = LocalizationSettings.StringDatabase.GetTable(tableType.ToString());
                if (tableStory == null)
                {
                    Debug.LogError($"无法获取本地化表: {tableType}");
                    return;
                }
                // Debug.Log($"成功获取本地化表: {tableType}");
            }
            catch (Exception e)
            {
                Debug.LogError($"获取本地化表时发生错误: {e.Message}\n{e.StackTrace}");
            }
        }




        /// <summary>
        /// 根据语言代码切换语言
        /// </summary>
        private void SwitchLanguage(string localeCode)
        {
            Locale newLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
            if (newLocale != null)
            {
                OnLocaleChanged(newLocale);
            }
            else
            {
                Debug.LogError($"Locale with code {localeCode} not found.");
            }
        }




        ////////////////////////////////////公共方法//////////////////////////////////////

        /// <summary>
        /// 获取本地化文本，是主要被外部调用的方法
        /// 如果文本键值不存在或当前语言没有翻译，会自动添加到本地化表中
        /// </summary>
        public string Translate(string key)
        {
            if (!enable)
            {
                return key;
            }
            try
            {
                if (tableStory == null)
                {
                    Debug.LogError("本地化表未初始化！");
                    return key;
                }

                var entry = tableStory.GetEntry(key);
                if (entry == null)
                {
                    Debug.LogWarning($"本地化键值 '{key}' 不存在，尝试添加新条目");
                    // 如果条目不存在，添加新条目
                    var newEntry = tableStory.AddEntry(key, $"[待翻译为{LocalizationSettings.SelectedLocale.Identifier.Code}]{key}");
                    Debug.LogWarning($"添加新的本地化键值: {key}");
                    return Translate(key);
                }

                string localizedString = entry.GetLocalizedString();
                if (string.IsNullOrEmpty(localizedString))
                {
                    Debug.LogWarning($"键值 '{key}' 在当前语言({LocalizationSettings.SelectedLocale.Identifier.Code})下没有翻译，已添加待翻译标记");
                    // 如果当前语言没有翻译，也添加待翻译标记
                    entry.Value = $"[待翻译为{LocalizationSettings.SelectedLocale.Identifier.Code}]{key}";
                    return Translate(key);
                }
                return localizedString;
            }
            catch (Exception e)
            {
                Debug.LogError($"本地化键值 '{key}' 处理时发生错误: {e.Message}\n{e.StackTrace}");
                return key; // 发生错误时返回原始文本
            }
        }


        /// <summary>
        /// 切换到中文
        /// </summary>
        public void SwitchToChinese()
        {
            SwitchLanguage("zh-Hans");
        }

        /// <summary>
        /// 切换到日文
        /// </summary>
        public void SwitchToJapanese()
        {
            SwitchLanguage("ja");
        }

        /// <summary>
        /// 切换到英文
        /// </summary>
        public void SwitchToEnglish()
        {
            SwitchLanguage("en");
        }


        public string GetCurrentLanguage()
        {
            return LocalizationSettings.SelectedLocale.Identifier.Code;
        }

    }
}
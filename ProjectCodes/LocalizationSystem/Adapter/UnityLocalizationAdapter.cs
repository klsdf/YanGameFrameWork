/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-21 14:25
 * Description: unity官方的本地化框架的适配器
 ****************************************************************************/


using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization;
using System;
using UnityEngine;
using System.Collections.Generic;


namespace YanGameFrameWork.LocalizationSystem
{
    /// <summary>
    /// unity官方的本地化框架的适配器
    /// </summary>
    [Serializable]
    public class UnityLocalizationAdapter : ILocalizationAdapter
    {
        public string tableName;

        private StringTable _table;
        private StringTable Table
        {
            get
            {
                if (_table == null)
                {
                    _table = LocalizationSettings.StringDatabase.GetTable(tableName);
                }
                return _table;
            }
            set
            {
                _table = value;
            }
        }

        private static readonly Dictionary<LanguageType, string> _anguageTypeToCode = new Dictionary<LanguageType, string>
        {
            { LanguageType.SimplifiedChinese, "zh-Hans" },
            { LanguageType.TraditionalChinese, "zh-Hant" },
            { LanguageType.Japanese, "ja" },
            { LanguageType.English, "en" }
        };

        private string GetLanguageCode(LanguageType language)
        {
            if (_anguageTypeToCode.TryGetValue(language, out var code))
                return code;
            Debug.LogError($"语言类型 {language} 不存在");
            return "zh-Hans";
        }

        private LanguageType GetLanguageType(string languageCode)
        {
            foreach (var pair in _anguageTypeToCode)
            {
                if (pair.Value == languageCode)
                    return pair.Key;
            }
            return LanguageType.SimplifiedChinese;
        }

        public UnityLocalizationAdapter(string tableName)
        {
            this.tableName = tableName;
            Init();
        }


        public void Init()
        {

            if (LocalizationSettings.AvailableLocales.Locales.Count > 0)
            {
                GetLocalizationTable(tableName);
            }
            else
            {
                LocalizationSettings.InitializationOperation.Completed += OnLocalizationInitialized;
            }
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
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
                GetLocalizationTable(tableName);
            }
            else
            {
                Debug.LogError("Localization initialization failed.");
            }
        }

        /// <summary>
        /// 获取本地化表
        /// </summary>
        private void GetLocalizationTable(string tableName)
        {
            try
            {
                Table = LocalizationSettings.StringDatabase.GetTable(tableName);
                if (Table == null)
                {
                    Debug.LogError($"无法获取本地化表: {tableName}");
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
        /// 获取本地化文本，是主要被外部调用的方法
        /// 如果文本键值不存在或当前语言没有翻译，会自动添加到本地化表中
        /// </summary>
        public string GetText(string key, MetaData metaData = null, string chineseText = null)
        {
            try
            {
                if (Table == null)
                {
                    Debug.LogError("本地化表未初始化！");
                    return key;
                }

                var entry = Table.GetEntry(key);
                if (entry == null)
                {
                    Debug.LogWarning($"本地化键值 '{key}' 不存在，尝试添加新条目");
                    // 如果条目不存在，添加新条目
                    var newEntry = Table.AddEntry(key, $"[待翻译为{LocalizationSettings.SelectedLocale.Identifier.Code}]{key}");
                    Debug.LogWarning($"添加新的本地化键值: {key}");
                    return GetText(key);
                }

                string localizedString = entry.GetLocalizedString();
                if (string.IsNullOrEmpty(localizedString))
                {
                    Debug.LogWarning($"键值 '{key}' 在当前语言({LocalizationSettings.SelectedLocale.Identifier.Code})下没有翻译，已添加待翻译标记");
                    // 如果当前语言没有翻译，也添加待翻译标记
                    entry.Value = $"[待翻译为{LocalizationSettings.SelectedLocale.Identifier.Code}]{key}";
                    return GetText(key);
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
        /// 根据语言代码切换语言
        /// </summary>
        public void SwitchLanguage(LanguageType language)
        {
            Locale newLocale = LocalizationSettings.AvailableLocales.GetLocale(GetLanguageCode(language));
            if (newLocale != null)
            {
                OnLocaleChanged(newLocale);
            }
            else
            {
                Debug.LogError($"Locale with code {GetLanguageCode(language)} not found.");
            }
        }


        public LanguageType GetCurrentLanguage()
        {
            return GetLanguageType(LocalizationSettings.SelectedLocale.Identifier.Code);
        }

        public void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }


        /// <summary>
        /// 切换语言
        /// </summary>
        private void OnLocaleChanged(Locale newLocale)
        {
            Debug.Log("切换语言：" + newLocale.Identifier.Code);
            LocalizationSettings.SelectedLocale = newLocale;
            //这里临时用一下Card
            GetLocalizationTable(tableName);
        }



    }
}
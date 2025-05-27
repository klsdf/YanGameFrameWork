/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-21 14:25
 * Description: 自定义的本地化框架的适配器
 ****************************************************************************/

using System.IO;
using UnityEngine;
using System.Collections.Generic;


namespace YanGameFrameWork.LocalizationSystem
{
    public class CustomLocalizationAdapter : ILocalizationAdapter
    {

        private string _defaultTableName = "Default";

        /// <summary>
        /// 缓存表
        /// 第一个是tableName
        /// 第二个是key
        /// 第三个是language
        /// </summary>
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> _tableCache
            = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();



        private void CacheTranslatedText(string tableName, string key, string language, string translatedText)
        {
            if (!_tableCache.TryGetValue(tableName, out var keyDict))
            {
                keyDict = new Dictionary<string, Dictionary<string, string>>();
                _tableCache[tableName] = keyDict;
            }

            if (!keyDict.TryGetValue(key, out var langDict))
            {
                langDict = new Dictionary<string, string>();
                keyDict[key] = langDict;
            }

            langDict[language] = translatedText;
        }

        private string GetCachedTranslatedText(string tableName, string key, string language)
        {
            if (_tableCache.TryGetValue(tableName, out var keyDict) &&
                keyDict.TryGetValue(key, out var langDict) &&
                langDict.TryGetValue(language, out var translatedText))
            {
                return translatedText;
            }
            return null;
        }


        public string GetFilePath(string tableName)
        {
            return Path.Combine(Application.streamingAssetsPath, $"Localization/{tableName}.csv");
        }

        private bool IsKeyValid(string key)
        {
            string pattern = @"^[A-Za-z]+_[A-Za-z0-9_\u4e00-\u9fa5]+_[A-Za-z0-9_\u4e00-\u9fa5]+(Text|Image)$";
            return System.Text.RegularExpressions.Regex.IsMatch(key, pattern);
        }

        private string GetTableNameFromKey(string key)
        {
            // 只判断是否以英文单词+下划线开头
            var match = System.Text.RegularExpressions.Regex.Match(key, @"^([A-Za-z]+)_");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return _defaultTableName;
        }

        public string GetText(string key, MetaData metaData, string chineseText = null)
        {
            string tableName = GetTableNameFromKey(key);


            LanguageType currentLanguage = YanGF.Localization.CurrentLanguageType;


            //先从缓存中获取
            string translatedText = GetCachedTranslatedText(tableName, key, currentLanguage.ToString());
            if (translatedText != null)
            {
                return translatedText;
            }


            string filePath = GetFilePath(tableName);
            var record = CSVReader.FindRecordByFieldValue(filePath, "key", key);
            if (record != null)
            {
                translatedText = record[currentLanguage.ToString()];
                //缓存翻译结果
                CacheTranslatedText(tableName, key, currentLanguage.ToString(), translatedText);
                return translatedText;
            }
            else
            {
                WriteToBeTranslatedRecord(key, metaData, filePath, chineseText);
                return GetText(key, metaData, chineseText);
            }
        }


        /// <summary>
        /// 写入待翻译记录
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="comment">注释</param>
        public void WriteToBeTranslatedRecord(string key, MetaData metaData, string filePath, string chineseText)
        {

            if (!IsKeyValid(key))
            {
                YanGF.Debug.LogWarning(nameof(CustomLocalizationAdapter), $"{metaData.className}.{metaData.methodName} (行号 {metaData.lineNumber}) key不符合命名规范: {key}，请参考：类型_所属单位_Text|Image的功能");
            }

            var fields = new List<string>
            {
                "key",
                LanguageType.SimplifiedChinese.ToString(),
                LanguageType.English.ToString(),
                LanguageType.TraditionalChinese.ToString(),
                LanguageType.Japanese.ToString(),
                "调用信息",
                "注释"
            };

            var data = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> {
                    { "key", key },
                    { LanguageType.SimplifiedChinese.ToString(), string.IsNullOrEmpty(chineseText) ? $"[待翻译为zhs]{key}" : chineseText },
                    { LanguageType.TraditionalChinese.ToString(), $"[待翻译为zht]{key}" },
                    { LanguageType.Japanese.ToString(), $"[待翻译为ja]{key}" },
                    { LanguageType.English.ToString(), $"[待翻译为en]{key}" },
                    { "调用信息", $"{metaData.className}.{metaData.methodName} (行号 {metaData.lineNumber})" },
                },
            };
            CSVWriter.WriteCSV(filePath, fields, data);
            YanGF.Debug.Log(nameof(CustomLocalizationAdapter), $"添加新的本地化键值:{key}");
        }

        public void SwitchLanguage(LanguageType language)
        {

        }

        // public LanguageType GetCurrentLanguage()
        // {
        //     return _currentLanguageType;
        // }

        public CustomLocalizationAdapter()
        {
        }

        public void OnLocaleChanged()
        {

        }

    }

}
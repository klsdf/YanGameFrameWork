/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-21 14:25
 * Description: 自定义的本地化框架的适配器
 ****************************************************************************/

using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;


namespace YanGameFrameWork.LocalizationSystem
{
    public class CustomLocalizationAdapter : ILocalizationAdapter
    {
        private LanguageType _currentLanguageType;


        private string _defaultTableName = "Default";

        public string GetFilePath(string tableName)
        {
            return Path.Combine(Application.streamingAssetsPath, $"{tableName}.csv");
        }

        private bool IsKeyValid(string key)
        {
            string pattern = @"^[A-Za-z]+_[A-Za-z0-9]+_[A-Za-z0-9]+(Text|Image)$";
            return System.Text.RegularExpressions.Regex.IsMatch(key, pattern);
        }

        private string GetTableNameFromKey(string key)
        {
            if (IsKeyValid(key))
            {
                int idx = key.IndexOf('_');
                if (idx > 0)
                    return key.Substring(0, idx);
            }
            return _defaultTableName;
        }

        public string Translate(string key, MetaData metaData, string chineseText = null)
        {
            string tableName = GetTableNameFromKey(key);
            string filePath = GetFilePath(tableName);

            if (!IsKeyValid(key))
            {
                YanGF.Debug.LogWarning(nameof(CustomLocalizationAdapter), $"{metaData.className}.{metaData.methodName} (行号 {metaData.lineNumber}) key不符合命名规范: {key}，请参考：类型_所属单位_Text|Image的功能");
            }

            var record = CSVReader.FindRecordByFieldValue(filePath, "key", key);
            if (record != null)
            {
                LanguageType currentLanguage = GetCurrentLanguage();
                return record[currentLanguage.ToString()];
            }
            else
            {
                WriteToBeTranslatedRecord(key, metaData, filePath, chineseText);
                return Translate(key, metaData, chineseText);
            }
        }


        /// <summary>
        /// 写入待翻译记录
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="comment">注释</param>
        public void WriteToBeTranslatedRecord(string key, MetaData metaData, string filePath, string chineseText)
        {

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
            _currentLanguageType = language;
        }

        public LanguageType GetCurrentLanguage()
        {
            return _currentLanguageType;
        }

        public CustomLocalizationAdapter()
        {
        }


        public void OnDestroy()
        {

        }

        public void OnLocaleChanged()
        {

        }
    }

}
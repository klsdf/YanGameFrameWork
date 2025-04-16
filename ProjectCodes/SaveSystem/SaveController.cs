/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-28
 * Description: 游戏存档系统，用于保存和读取游戏数据
 *
 * 修改记录：
 * 2025-04-11 闫辰祥 允许保存的文件可以有多个并且可以自定义名字
 *
 ****************************************************************************/
using UnityEngine;
using System;
using YanGameFrameWork.CoreCodes;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using YanGameFrameWork.Editor;


namespace YanGameFrameWork.SaveSystem
{
    public class SaveController : Singleton<SaveController>
    {
        private string _defaultSaveFilePath = "savefile.json";
        private string _customSaveFilePath;

        public string SaveFilePath
        {
            get
            {
                if (_customSaveFilePath == null || _customSaveFilePath == "")
                {
                    return Path.Combine(Application.persistentDataPath, _defaultSaveFilePath);
                }
                else
                {
                    return Path.Combine(Application.persistentDataPath, _customSaveFilePath);
                }
            }
        }

        [Header("保存数据列表")]
        [SerializeField]
        private List<SaveData> _saveDataList = new List<SaveData>();


        public SaveController SetSaveFilePath(string saveFilePath)
        {
            _customSaveFilePath = saveFilePath;
            return this;
        }

        public SaveController SetDefaultSaveFilePath()
        {
            _customSaveFilePath = _defaultSaveFilePath;
            return this;
        }



        /// <summary>
        /// 保存数据
        /// TODO: 要多个文件
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="data">数据</param>
        public void Save<T>(string key, T data)
        {
            try
            {
                // 确保目录存在
                string directory = Path.GetDirectoryName(SaveFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 如果文件已存在，先读取旧数据
                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath);
                    _saveDataList = JsonUtility.FromJson<SaveDataList>(json).DataList;
                }
                else
                {
                    _saveDataList = new List<SaveData>();
                }

                // 检查是否已存在相同 key 的数据，若存在则更新
                bool found = false;
                string jsonData;

                // 处理基本类型
                if (data is ValueType || data is string)
                {
                    jsonData = data.ToString();
                }
                else
                {
                    jsonData = JsonUtility.ToJson(data);

                    if (data is Array)
                    {
                        print($"data is Array:{data.ToString()}");
                    }
                    print($"jsonData:{jsonData}");
                }

                foreach (var item in _saveDataList)
                {
                    if (item.Key == key)
                    {
                        item.JsonData = jsonData;
                        found = true;
                        break;
                    }
                }

                // 如果 key 不存在，则添加新数据
                if (!found)
                {
                    _saveDataList.Add(new SaveData { Key = key, JsonData = jsonData });
                }

                // 序列化并写入文件
                string updatedJson = JsonUtility.ToJson(new SaveDataList { DataList = _saveDataList }, true);
                File.WriteAllText(SaveFilePath, updatedJson);
                YanGF.Debug.Log(nameof(SaveController), $"保存成功，存储路径：{SaveFilePath}，存储字段{key},数据：{jsonData}");
            }
            catch (Exception e)
            {
                YanGF.Debug.LogError(nameof(SaveController), $"保存失败：{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>数据</returns>
        public T Load<T>(string key, T defaultValue = default)
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath);
                    _saveDataList = JsonUtility.FromJson<SaveDataList>(json).DataList;

                    foreach (var item in _saveDataList)
                    {
                        if (item.Key == key)
                        {
                            // 处理基本类型
                            if (typeof(T) == typeof(int))
                            {
                                return (T)(object)int.Parse(item.JsonData);
                            }
                            else if (typeof(T) == typeof(float))
                            {
                                return (T)(object)float.Parse(item.JsonData);
                            }
                            else if (typeof(T) == typeof(bool))
                            {
                                return (T)(object)bool.Parse(item.JsonData);
                            }
                            else if (typeof(T) == typeof(string))
                            {
                                return (T)(object)item.JsonData;
                            }
                            else
                            {
                                // 复杂类型使用 JsonUtility
                                return JsonUtility.FromJson<T>(item.JsonData);
                            }
                        }
                    }
                    YanGF.Debug.LogWarning(nameof(SaveController), $"未找到键为 {key} 的数据。");
                }
                else
                {
                    YanGF.Debug.LogWarning(nameof(SaveController), "保存文件不存在。");
                }
            }
            catch (Exception e)
            {
                YanGF.Debug.LogError(nameof(SaveController), $"加载数据失败：{e.Message}\n{e.StackTrace}");
            }
            return defaultValue;
        }


        /// <summary>
        /// 打开保存目录
        /// </summary>
        [Button("打开保存目录")]
        public void OpenSaveDirectory()
        {
            try
            {
                string directory = Path.GetDirectoryName(SaveFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Windows 系统
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    Process.Start("explorer.exe", directory);
                }
                // macOS 系统
                else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                {
                    Process.Start("open", directory);
                }
                // Linux 系统
                else if (Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.LinuxPlayer)
                {
                    Process.Start("xdg-open", directory);
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"当前平台 {Application.platform} 不支持直接打开文件夹");
                }

                UnityEngine.Debug.Log($"保存目录：{directory}");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"打开保存目录失败：{e.Message}\n{e.StackTrace}");
            }
        }


        [Button("保存数据")]
        public void TestSave()
        {
            YanGF.Save.Save("SkillSystemUnlockedNodes", new string[] { "123", "234", "234234" });
        }

    }




}

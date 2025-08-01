using UnityEngine;
using System.Collections.Generic;
using System.IO;
using YanGameFrameWork.Editor;



public class ReadConfigTableExample : MonoBehaviour
{
    [Header("配置表文件")]
    [SerializeField] private TextAsset configFile; // 在Inspector中拖入文本文件

    class LoliData
    {
        public string name;
        public int age;
        public string description;
    }

    [Button("测试读取配置表通过路径")]
    public void TestReadConfigTableByPath()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "LoliData.csv");
        List<LoliData> data = ConfigTableReader.ReadConfigTableByPath<LoliData>(filePath);
        foreach (var item in data)
        {
            Debug.Log($"name: {item.name}, age: {item.age}, description: {item.description}");
        }
    }

    [Button("测试读取配置表通过文件")]
    public void TestReadConfigTableByFile()
    {
        if (configFile == null)
        {
            Debug.LogError("请在Inspector中拖入配置表文件！");
            return;
        }

        string fileContent = configFile.text;
        List<LoliData> data = ConfigTableReader.ReadConfigTableByFile<LoliData>(fileContent);

        if (data != null)
        {
            Debug.Log($"成功读取到 {data.Count} 条数据：");
            foreach (var item in data)
            {
                Debug.Log($"name: {item.name}, age: {item.age}, description: {item.description}");
            }
        }
        else
        {
            Debug.LogError("读取配置表失败！");
        }
    }



}


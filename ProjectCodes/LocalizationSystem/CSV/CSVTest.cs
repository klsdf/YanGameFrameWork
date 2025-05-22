using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.IO;

public class CSVTest : MonoBehaviour
{
    public string tableName;
    public string FilePath
    {
        get
        {
            return Path.Combine(Application.streamingAssetsPath, tableName + ".csv");
        }
    }
    [Button("测试")]
    public void TestWriteField()
    {
        var fields = new List<string> { "key", "zh", "en", "ja" };
        var data = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "key", "UI_StartPanel_StartGameText" }, { "zh", "开始游戏" }, { "en", "Start Game" }, { "ja", "ゲームを開始" } },
            new Dictionary<string, string> { { "key", "UI_StartPanel_EndGameText" }, { "zh", "结束游戏" }, { "en", "End Game" }, { "ja", "ゲームを終了" } }
        };
        CSVWriter.WriteCSV(FilePath, fields, data);
        Debug.Log("写入成功");
    }


    // [Button("测试读取")]
    // public void TestReadField()
    // {
    //     // 示例：获取第1条记录的"zh"字段内容
    //     string zhValue = CSVReader.ReadCSV(FilePath)[0]["zh"];
    //     Debug.Log("第1条记录的中文内容: " + zhValue);

    //     // 示例：获取第2条记录的"en"字段内容
    //     string enValue = CSVReader.ReadCSV(FilePath)[1]["en"];
    //     Debug.Log("第2条记录的英文内容: " + enValue);

    //     Debug.Log("读取成功");
    // }

    [Button("测试查找记录")]
    public void TestFindRecord()
    {
        var record = CSVReader.FindRecordByFieldValue(FilePath, "key", "UI_StartPanel_StartGameText");
        if (record != null)
        {
            // 处理record
            Debug.Log("找到记录: " + string.Join(", ", record));
        }
        else
        {
            Debug.Log("未找到记录");
        }
    }



    [Button("测试查找记录2")]
    public void TestFindRecordByFieldValue()
    {
        var record = CSVReader.FindRecordByFieldValue(FilePath, "key", "123456");
        if (record != null)
        {
            Debug.Log("找到记录: " + string.Join(", ", record));
        }
        else
        {
            Debug.Log("未找到记录");
        }
    }



    [Button("测试读取记录")]
    public void TestReadRecord()
    {
        string key = "key";
        string field = @"<align=""center""><b>基础操作</b>

鼠标悬停在单位或者页面中的各个UI上查看详情页面。

左键拖动单位。

右键移动整个地图。

滚轮控制视角缩放。";
        var record = CSVReader.FindRecordByFieldValue(FilePath, key, field);
        if (record != null)
        {
            Debug.Log("记录: " + string.Join(", ", record));
        }
        else
        {
            Debug.Log("未找到记录");
        }
    }



    [Button("测试读取列")]
    public void TestReadColumn()
    {
        CSVReader.PrintAllValuesOfField(FilePath, "key");


    }
}

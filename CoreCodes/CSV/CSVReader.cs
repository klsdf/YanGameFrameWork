/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-20
 * Description: csv的读取
 ****************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Globalization;

public static class CSVReader
{


    /// <summary>
    /// 根据字段名和字段值查找记录
    /// </summary>
    /// <param name="filePath">csv文件路径</param>
    /// <param name="fieldName">字段名</param>
    /// <param name="fieldValue">字段值</param>
    /// <returns>记录</returns>
    public static Dictionary<string, string> FindRecordByFieldValue(string filePath, string fieldName, string fieldValue)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"文件不存在: {filePath}");
            return null;
        }

        string fileContent = File.ReadAllText(filePath, Encoding.UTF8);
        List<string[]> parsedData = ParseCSV(fileContent);

        if (parsedData.Count < 1)
        {
            Debug.LogError("CSV文件为空");
            return null;
        }

        string[] headers = parsedData[0];
        int targetColumnIndex = -1;

        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i] == fieldName)
            {
                targetColumnIndex = i;
                break;
            }
        }

        if (targetColumnIndex == -1)
        {
            Debug.LogError($"字段名 '{fieldName}' 不存在");
            return null;
        }

        for (int i = 1; i < parsedData.Count; i++)
        {
            string[] row = parsedData[i];
            if (row.Length > targetColumnIndex)
            {
                // 如果字段值是多行，则将多行内的各种换行符统一为\n
                string a = row[targetColumnIndex].Trim().Replace("\r\n", "\n").Replace("\r", "\n");
                string b = fieldValue.Trim().Replace("\r\n", "\n").Replace("\r", "\n");
                // Debug.Log($"比较: [{a}] vs [{b}]");
                if (a == b)
                {
                    Dictionary<string, string> record = new Dictionary<string, string>();
                    for (int j = 0; j < headers.Length && j < row.Length; j++)
                    {
                        record[headers[j]] = row[j];
                    }
                    return record;
                }
            }
        }

        Debug.Log($"未找到字段值 '{fieldValue}' 的记录");
        return null;
    }



    /// <summary>
    /// 读取csv文件
    /// </summary>
    /// <param name="csvContent">csv文件内容</param>
    /// <returns>csv文件内容，每一行为一个字符串数组</returns>
    private static List<string[]> ParseCSV(string csvContent)
    {
        List<string[]> result = new List<string[]>();
        List<string> currentRow = new List<string>();
        StringBuilder currentField = new StringBuilder();
        bool inQuotes = false;
        bool isEscaped = false;

        for (int i = 0; i < csvContent.Length; i++)
        {
            char c = csvContent[i];

            if (isEscaped)
            {
                currentField.Append(c);
                isEscaped = false;
                continue;
            }

            switch (c)
            {
                case '"':
                    if (inQuotes && i + 1 < csvContent.Length && csvContent[i + 1] == '"')
                    {
                        // 转义引号
                        currentField.Append('"');
                        i++; // 跳过下一个引号
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                    break;

                case ',':
                    if (inQuotes)
                    {
                        currentField.Append(c);
                    }
                    else
                    {
                        currentRow.Add(currentField.ToString());
                        currentField.Clear();
                    }
                    break;

                case '\n':
                case '\r':
                    if (inQuotes)
                    {
                        currentField.Append(c);
                    }
                    else
                    {
                        if (c == '\n' || (c == '\r' && (i + 1 >= csvContent.Length || csvContent[i + 1] != '\n')))
                        {
                            currentRow.Add(currentField.ToString());
                            result.Add(currentRow.ToArray());
                            currentRow.Clear();
                            currentField.Clear();
                        }
                    }
                    break;

                default:
                    currentField.Append(c);
                    break;
            }
        }

        // 添加最后一个字段和行
        if (currentField.Length > 0 || currentRow.Count > 0)
        {
            currentRow.Add(currentField.ToString());
            result.Add(currentRow.ToArray());
        }

        return result;
    }

    /// <summary>
    /// 打印所有字段值
    /// </summary>
    /// <param name="filePath">csv文件路径</param>
    /// <param name="fieldName">字段名</param>
    public static void PrintAllValuesOfField(string filePath, string fieldName)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"文件不存在: {filePath}");
            return;
        }

        string fileContent = File.ReadAllText(filePath, Encoding.UTF8);
        List<string[]> parsedData = ParseCSV(fileContent);

        if (parsedData.Count < 1)
        {
            Debug.LogError("CSV文件为空");
            return;
        }

        string[] headers = parsedData[0];
        int targetColumnIndex = -1;

        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i] == fieldName)
            {
                targetColumnIndex = i;
                break;
            }
        }

        if (targetColumnIndex == -1)
        {
            Debug.LogError($"字段名 '{fieldName}' 不存在");
            return;
        }

        Debug.Log($"字段 '{fieldName}' 的所有记录：");
        for (int i = 1; i < parsedData.Count; i++)
        {
            string[] row = parsedData[i];
            if (row.Length > targetColumnIndex)
            {
                Debug.Log(row[targetColumnIndex]);
            }
        }
    }

    /// <summary>
    /// 读取CSV文件的路径，返回每一行的字段字典列表
    /// </summary>
    /// <param name="filePath">csv文件路径</param>
    /// <returns>每一行为一个字符串数组</returns>
    public static List<string[]> CheckAndRead(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"文件不存在: {filePath}");
            return null;
        }

        string fileContent = File.ReadAllText(filePath, Encoding.UTF8);
        if (fileContent == null)
        {
            Debug.LogError($"文件内容为空: {filePath}");
            return null;
        }
        List<string[]> result = ParseCSV(fileContent);
        if (result.Count < 1)
        {
            Debug.LogError("CSV文件为空");
            return null;
        }
        return result;
    }


    public static List<string[]> ReadFile(string fileContent)
    {
        List<string[]> result = ParseCSV(fileContent);
        if (result.Count < 1)
        {
            Debug.LogError("CSV文件为空");
            return null;
        }
        return result;
    }




    public static List<Dictionary<string, object>> ReadAsDictionaryWithType(string filePath)
    {
        var rows = CSVReader.CheckAndRead(filePath);

        if (rows == null || rows.Count < 3) return null;

        var headers = rows[0];
        var types = rows[1];
        var result = new List<Dictionary<string, object>>();

        for (int i = 2; i < rows.Count; i++)
        {
            var row = rows[i];
            var dict = new Dictionary<string, object>();
            for (int j = 0; j < headers.Length && j < row.Length; j++)
            {
                string type = types[j].ToLower();
                string value = row[j];
                object realValue = value;
                switch (type)
                {
                    case "int":
                        if (int.TryParse(value, out int intVal)) realValue = intVal;
                        break;
                    case "float":
                        if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatVal)) realValue = floatVal;
                        break;
                    case "double":
                        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleVal)) realValue = doubleVal;
                        break;
                    case "bool":
                        if (bool.TryParse(value, out bool boolVal)) realValue = boolVal;
                        break;
                    case "string":
                    default:
                        realValue = value;
                        break;
                }
                dict[headers[j]] = realValue;
            }
            result.Add(dict);
        }
        return result;
    }


}

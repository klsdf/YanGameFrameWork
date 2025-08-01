using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using System;

public static class ConfigTableReader
{


    public static List<T> ReadConfigTableByPath<T>(string filePath) where T : new()
    {
        var rows = CSVReader.CheckAndRead(filePath);
        return ParseRows<T>(rows);
    }

    public static List<T> ReadConfigTableByFile<T>(string fileContent) where T : new()
    {
        var rows = CSVReader.ReadFile(fileContent);
        return ParseRows<T>(rows);
    }




    private static List<T> ParseRows<T>(List<string[]>  rows) where T : new()
    {

        // foreach (var row in rows)
        // {
        //     for (int i = 0; i < row.Length; i++)
        //     {
        //         Debug.Log(row[i]);
        //     }
        //     Debug.Log("--------------------------------");
        // }

        if (rows == null || rows.Count <= 3) return null;


        //数据名称
        string[] headers = rows[0];

        //数据类型
        string[] types = rows[1];

        //数据注释
        string[] comments = rows[2];

        var result = new List<T>();

        for (int i = 3; i < rows.Count; i++)
        {
            var row = rows[i];
            var obj = new T();

            for (int j = 0; j < headers.Length && j < row.Length; j++)
            {
                string header = headers[j];
                string type = types[j].ToLower();
                string value = row[j];


                PropertyInfo prop = typeof(T).GetProperty(header, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null && prop.CanWrite)
                {
                    var targetType = prop.PropertyType;
                    object realValue = ConvertToType(value, targetType);
                    try { prop.SetValue(obj, realValue); } catch { Debug.LogError("设置属性失败: " + header); }
                }
                else
                {
                    FieldInfo field = typeof(T).GetField(header, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (field != null)
                    {
                        var targetType = field.FieldType;
                        object realValue = ConvertToType(value, targetType);
                        try { field.SetValue(obj, realValue); } catch { Debug.LogError("设置字段失败: " + header); }
                    }
                }

            }

            result.Add(obj);
        }
        return result;
    }


    private static object ConvertToType(string value, Type targetType)
    {
        if (targetType == typeof(int))
        {
            return int.Parse(value);
        }
        if (targetType == typeof(float))
        {
            return float.Parse(value);
        }
        if (targetType == typeof(double))
        {
            return double.Parse(value);
        }
        if (targetType == typeof(bool))
        {
            return bool.Parse(value);
        }
        if (targetType == typeof(string))
        {
            return value;
        }
        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, value);
        }
        if (targetType == typeof(ValueTuple<float, float, float>))
        {
            var parts = value.Split('|');
            if (parts.Length == 3 &&
                float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var v1) &&
                float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var v2) &&
                float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var v3))
            {
                return (v1, v2, v3);
            }
            Debug.LogWarning($"元组字段解析失败，值：{value}");
            return (0f, 0f, 0f);
        }
        // 其他类型可扩展
        return value;
    }
}

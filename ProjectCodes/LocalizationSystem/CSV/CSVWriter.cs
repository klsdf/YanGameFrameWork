/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-19
 * Description: csv的写入
 ****************************************************************************/
using System.Collections.Generic;
using System.IO;
using System.Text;


public static class CSVWriter
{
    /// <summary>
    /// 写入CSV文件
    /// </summary>
    /// <param name="filePath">CSV文件路径</param>
    /// <param name="fieldNames">字段名列表（表头）</param>
    /// <param name="data">数据，每行为一个字典，key为字段名，value为字段值</param>
    public static void WriteCSV(string filePath, List<string> fieldNames, List<Dictionary<string, string>> data)
    {
        // 确保目录存在
        string directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        bool fileExists = File.Exists(filePath);
        using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            // 只有文件不存在时才写表头
            if (!fileExists)
                writer.WriteLine(string.Join(",", fieldNames.ConvertAll(EscapeCSV)));

            // 写入数据
            foreach (var row in data)
            {
                List<string> line = new List<string>();
                foreach (var field in fieldNames)
                {
                    row.TryGetValue(field, out string value);
                    line.Add(EscapeCSV(value ?? ""));
                }
                writer.WriteLine(string.Join(",", line));
            }
        }
    }

    // 处理CSV特殊字符
    private static string EscapeCSV(string value)
    {
        if (value == null)
            return "";
        bool mustQuote = value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r");
        if (mustQuote)
        {
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\"";
        }
        return value;
    }


}


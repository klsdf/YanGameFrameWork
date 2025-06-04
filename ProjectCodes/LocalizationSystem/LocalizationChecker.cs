using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using JetBrains.Annotations;
using System.Linq;

public class LocalizationChecker
{
    // 新增：本地化方法名（可根据实际项目扩展）
    private static readonly string[] LocalizationMethods = { "Translate" };
    // 新增：正则表达式，匹配字符串字面量
    private static readonly Regex StringLiteralRegex = new Regex(
        @"""([^""\\]*(?:\\.[^""\\]*)*)""",
        RegexOptions.Compiled | RegexOptions.Multiline
    );

    static LocalizationChecker()
    {
        // CompilationPipeline.assemblyCompilationFinished += CheckHelloWorld;
    }
    public static void CheckLocalization(string folderPath)
    {

        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"文件夹路径不存在：{folderPath}");
            return;
        }
        var csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);
        foreach (var file in csFiles)
        {
            string content = File.ReadAllText(file);
            CheckHardcodedStringsInScript(file, content);
        }
    }

    public static void CheckHardcodedStringsInScript(string filePath, string content)
    {
        var matches = StringLiteralRegex.Matches(content);
        foreach (Match match in matches)
        {
            string strValue = match.Groups[1].Value; // 提取字符串内容（去除双引号）
            int lineNumber = GetLineNumber(content, match.Index); // 获取行号

            // 排除空字符串和仅有空白的字符串
            // 排除Debug.Log、Debug.LogWarning、Debug.LogError
            // 排除Header属性
            if (!string.IsNullOrWhiteSpace(strValue)
            && !IsInDebugLog(content, match.Index)
            && !IsInHeaderAttribute(content, match.Index))
            {
                YanGF.Debug.LogWarning(nameof(LocalizationChecker),
                $"[本地化警告] 文件：{filePath} 行号：{lineNumber}\n" +
                    $"未本地化字符串：\"{strValue}\"\n");
            }
        }
    }

    private static bool IsInDebugLog(string content, int stringIndex)
    {
        // 向前查找最近的"Debug.Log"等
        string[] logMethods = { "Debug.LogWarning", "Debug.LogError", "Debug.Log" };
        int nearestLog = -1;
        string foundMethod = null;
        foreach (var method in logMethods)
        {
            int idx = content.LastIndexOf(method, stringIndex);
            if (idx != -1 && (nearestLog == -1 || idx > nearestLog))
            {
                nearestLog = idx;
                foundMethod = method;
            }
        }
        if (nearestLog == -1)
            return false;

        // 检查stringIndex是否在该方法的括号内
        int openParen = content.IndexOf('(', nearestLog + foundMethod.Length);
        int closeParen = content.IndexOf(')', openParen);
        if (openParen == -1 || closeParen == -1)
            return false;
        return stringIndex > openParen && stringIndex < closeParen;
    }

    private static bool IsInHeaderAttribute(string content, int stringIndex)
    {
        // 向前查找最近的 [Header(
        int headerIdx = content.LastIndexOf("[Header(", stringIndex);
        if (headerIdx == -1)
            return false;
        // 找到最近的 ) 位置
        int closeBracketIdx = content.IndexOf(")", headerIdx);
        // 字符串字面量必须在 [Header( 和 ) 之间
        return closeBracketIdx != -1 && stringIndex > headerIdx && stringIndex < closeBracketIdx;
    }

    /// <summary>
    /// 根据内容和索引获取行号
    /// </summary>
    /// <param name="content">文件内容</param>
    /// <param name="index">匹配的索引</param>
    /// <returns>行号</returns>
    private static int GetLineNumber(string content, int index)
    {
        // 计算从开始到索引处的行数
        return content.Take(index).Count(c => c == '\n') + 1;
    }
}

// using UnityEditor;
// using System.IO;
// using System.Text.RegularExpressions;
// using UnityEngine;
// using JetBrains.Annotations;

// [InitializeOnLoad]
// public class LocalizationChecker
// {
//     // 新增：本地化方法名（可根据实际项目扩展）
//     private static readonly string[] LocalizationMethods = { "Translate" };
//     // 新增：正则表达式，匹配字符串字面量
//     private static readonly Regex StringLiteralRegex = new Regex(
//         @"""([^""\\]*(?:\\.[^""\\]*)*)""",
//         RegexOptions.Compiled | RegexOptions.Multiline
//     );

//     static LocalizationChecker()
//     {
//         // CompilationPipeline.assemblyCompilationFinished += CheckHelloWorld;
//     }

//     // private static void CheckHelloWorld(string assemblyPath, CompilerMessage[] messages)
//     // {
//     //     // 遍历所有cs文件
//     //     foreach (var file in Directory.GetFiles("Assets/Scripts", "*.cs", SearchOption.AllDirectories))
//     //     {
//     //         string content = File.ReadAllText(file);
//     //         // 原有Hello World检测
//     //         if (Regex.IsMatch(content, @"\bHello World\b", RegexOptions.IgnoreCase))
//     //         {
//     //             UnityEngine.Debug.LogWarning($"检测到硬编码的 'Hello World'！文件路径：{file}");
//     //         }
//     //     }
//     // }

//     public static void CheckLocalization()
//     {
//         string folderPath = "Assets/YanGameFrameWork";
//         var csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);
//         foreach (var file in csFiles)
//         {
//             string content = File.ReadAllText(file);
//             CheckHardcodedStringsInScript(file, content);
//         }
//     }



//     public static void CheckHardcodedStringsInScript(string filePath, string content)
//     {
//         var matches = StringLiteralRegex.Matches(content);
//         foreach (Match match in matches)
//         {
//             string strValue = match.Groups[1].Value; // 提取字符串内容（去除双引号）
//             int lineNumber = GetLineNumber(content, match.Index); // 获取行号

// //     // 新增：检测未本地化字符串
// //     private static void CheckHardcodedStringsInScript(string filePath, string content)
// //     {
// //         var matches = StringLiteralRegex.Matches(content);
// //         foreach (Match match in matches)
// //         {
// //             string strValue = match.Groups[1].Value; // 提取字符串内容（去除双引号）
// //             int lineNumber = GetLineNumber(content, match.Index); // 获取行号

//             // 排除空字符串和仅有空白的字符串
//             // 排除Debug.Log、Debug.LogWarning、Debug.LogError
//             // 排除Header属性
//             if (!isLocalized
//             && !string.IsNullOrWhiteSpace(strValue)
//             && !IsInDebugLog(content, match.Index)
//             && !IsInHeaderAttribute(content, match.Index))
//             {
//                 UnityEngine.Debug.LogWarning($"[本地化警告] 文件：{filePath}\n" +
//                     $"行号：{lineNumber}\n" +
//                     $"未本地化字符串：\"{strValue}\"\n" +
//                     "请使用本地化方法包裹（如：Translate(\"key\")）！");
//             }
//         }
//     }

// //             // 排除空字符串和仅有空白的字符串
// //             if (!isLocalized && !string.IsNullOrWhiteSpace(strValue) && !IsInDebugLog(content, match.Index))
// //             {
// //                 UnityEngine.Debug.LogWarning($"[本地化警告] 文件：{filePath}\n" +
// //                     $"行号：{lineNumber}\n" +
// //                     $"未本地化字符串：\"{strValue}\"\n" +
// //                     "请使用本地化方法包裹（如：Translate(\"key\")）！");
// //             }
// //         }
// //     }

// //     // 新增：判断字符串是否被本地化方法包裹
// //     private static bool IsStringLocalized(string content, string strValue)
// //     {
// //         foreach (var method in LocalizationMethods)
// //         {
// //             // 允许Translate("xxx")、Translate ( "xxx" )等格式
// //             string pattern = $@"{method}\s*\(\s*""{Regex.Escape(strValue)}""";
// //             if (Regex.IsMatch(content, pattern))
// //             {
// //                 return true;
// //             }
// //         }
// //         return false;
// //     }

//     private static bool IsInDebugLog(string content, int stringIndex)
//     {
//         // 向前查找最近的"Debug.Log"等
//         string[] logMethods = { "Debug.LogWarning", "Debug.LogError", "Debug.Log" };
//         int nearestLog = -1;
//         string foundMethod = null;
//         foreach (var method in logMethods)
//         {
//             int idx = content.LastIndexOf(method, stringIndex);
//             if (idx != -1 && (nearestLog == -1 || idx > nearestLog))
//             {
//                 nearestLog = idx;
//                 foundMethod = method;
//             }
//         }
//         if (nearestLog == -1)
//             return false;

//         // 检查stringIndex是否在该方法的括号内
//         int openParen = content.IndexOf('(', nearestLog + foundMethod.Length);
//         int closeParen = content.IndexOf(')', openParen);
//         if (openParen == -1 || closeParen == -1)
//             return false;
//         return stringIndex > openParen && stringIndex < closeParen;
//     }

//     private static bool IsInHeaderAttribute(string content, int stringIndex)
//     {
//         // 向前查找最近的 [Header(
//         int headerIdx = content.LastIndexOf("[Header(", stringIndex);
//         if (headerIdx == -1)
//             return false;
//         // 找到最近的 ) 位置
//         int closeBracketIdx = content.IndexOf(")", headerIdx);
//         // 字符串字面量必须在 [Header( 和 ) 之间
//         return closeBracketIdx != -1 && stringIndex > headerIdx && stringIndex < closeBracketIdx;
//     }
// }

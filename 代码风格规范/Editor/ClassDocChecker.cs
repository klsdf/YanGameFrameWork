using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
public static class ClassDocChecker
{

    // static ClassDocChecker()
    // {
    //     CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompiled;
    // }

    // private static void OnAssemblyCompiled(string assemblyPath, CompilerMessage[] messages)
    // {
    //     // 只检测主工程的代码
    //     if (!assemblyPath.Contains("Assembly-CSharp.dll"))
    //         return;

    //     // CheckClassDoc();
    // }


    [MenuItem("😁YanGameFrameWork😁/检查代码风格")]
    private static void CheckCodeStyle()
    {
        CheckClassDoc();
    }

    private static void CheckClassDoc()
    {
        string folderPath = "Assets/YanGameFrameWork";
        var csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

        bool allPassed = true;
        foreach (var file in csFiles)
        {
            string code = File.ReadAllText(file);
            // 跳过开头的空行
            var lines = code.Split('\n');
            int idx = 0;
            while (idx < lines.Length && string.IsNullOrWhiteSpace(lines[idx])) idx++;

            // 查找第一个多行注释
            if (idx < lines.Length && lines[idx].TrimStart().StartsWith("/*"))
            {
                var commentLines = new List<string>();
                while (idx < lines.Length)
                {
                    commentLines.Add(lines[idx]);
                    if (lines[idx].Contains("*/")) break;
                    idx++;
                }
                string comment = string.Join("\n", commentLines);
                if (!(comment.Contains("Author") && comment.Contains("Date")))
                {
                    Debug.LogError($"⛔ 缺少文档注释: 文件顶部注释不包含 Author 或 Date (at {file.Replace("\\", "/")})");
                    allPassed = false;
                }
            }
            else
            {
                Debug.LogError($"⛔ 缺少文档注释: 文件顶部没有多行注释 (at {file.Replace("\\", "/")})");
                allPassed = false;
            }
        }
        if (allPassed)
        {
            Debug.Log("✅ 所有文件的顶部文档注释都符合规范！");
        }
    }

}


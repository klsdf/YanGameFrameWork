#if UNITY_EDITOR
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public static class RoslynLibraryChecker
{
    // 定义符号，用于标识Roslyn库是否可用
    static string defineSymbol = "USE_ROSLYN";
    // 假设Roslyn库的路径（根据实际情况调整）
    static string roslynLibraryPath = "Packages/com.unity.roslyn/Editor/Roslyn.dll";

    static RoslynLibraryChecker()
    {
        bool hasRoslynLibrary = DoesRoslynLibraryExist();
        SetScriptingDefineSymbol(hasRoslynLibrary);
    }

    /// <summary>
    /// 检查Roslyn库是否存在
    /// </summary>
    /// <returns>如果存在返回true，否则返回false</returns>
    private static bool DoesRoslynLibraryExist()
    {
        return File.Exists(roslynLibraryPath);
    }

    /// <summary>
    /// 根据Roslyn库的存在与否设置脚本定义符号
    /// </summary>
    /// <param name="enabled">是否启用符号</param>
    private static void SetScriptingDefineSymbol(bool enabled)
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup);

        if (enabled && !defines.Contains(defineSymbol))
        {
            defines += $";{defineSymbol}";
        }
        else if (!enabled && defines.Contains(defineSymbol))
        {
            defines = defines.Replace(defineSymbol, "").Replace(";;", ";");
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup, defines);
    }
}
#endif 
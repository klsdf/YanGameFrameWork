/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-08
 *
 * Description: 用于检查LibTessDotNet.dll是否存在，并设置脚本定义符号。可以用#if USE_LIBTESSDOTNET来判断是否存在。
 * 这样即便某些库没有安装，也不会报错。
 ****************************************************************************/

using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public static class LibraryChecker
{

    static string defineSymbol = "USE_LIBTESSDOTNET";
    static string libraryPath = "Assets/Packages/LibTessDotNet.1.1.15/lib/netstandard2.0/LibTessDotNet.dll";
    static LibraryChecker()
    {
        bool hasLibrary = DoesLibraryExist();
        SetScriptingDefineSymbol(hasLibrary);
    }

    private static bool DoesLibraryExist()
    {
        return System.IO.File.Exists(libraryPath);
    }

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
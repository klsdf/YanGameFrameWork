/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-21
 *
 * Description: 用于检查Unity Localization包是否存在，并设置脚本定义符号。
 * 可以用#if UNITY_LOCALIZATION来判断是否存在。
 * 这样即便没有安装Localization包，也不会报错。
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.Linq;
using UnityEngine;

[InitializeOnLoad]
public static class LocalizationLibraryChecker
{
    /// <summary>
    /// 编译符号名称
    /// </summary>
    private const string DEFINE_SYMBOL = "UNITY_LOCALIZATION";

    /// <summary>
    /// Localization包的名称
    /// </summary>
    private const string PACKAGE_NAME = "com.unity.localization";

    /// <summary>
    /// 静态构造函数，在Unity编辑器启动时执行
    /// </summary>
    static LocalizationLibraryChecker()
    {
        // 延迟一帧执行，确保PackageManager已经初始化
        EditorApplication.delayCall += CheckLocalizationPackage;
    }

    /// <summary>
    /// 检查Localization包是否存在
    /// </summary>
    private static void CheckLocalizationPackage()
    {
        var listRequest = Client.List();
        while (!listRequest.IsCompleted)
        {
            // 等待包列表加载完成
        }

        if (listRequest.Status == StatusCode.Success)
        {
            bool hasLocalization = listRequest.Result.Any(package => package.name == PACKAGE_NAME);
            SetScriptingDefineSymbol(hasLocalization);
        }
        else
        {
            Debug.LogError("获取包列表失败，无法检查Localization包");
        }
    }

    /// <summary>
    /// 设置编译符号
    /// </summary>
    /// <param name="enabled">是否启用编译符号</param>
    private static void SetScriptingDefineSymbol(bool enabled)
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup);

        if (enabled && !defines.Contains(DEFINE_SYMBOL))
        {
            defines += $";{DEFINE_SYMBOL}";
            Debug.Log("已添加UNITY_LOCALIZATION编译符号");
        }
        else if (!enabled && defines.Contains(DEFINE_SYMBOL))
        {
            defines = defines.Replace(DEFINE_SYMBOL, "").Replace(";;", ";");
            Debug.Log("已移除UNITY_LOCALIZATION编译符号");
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup, defines);
    }
}
#endif
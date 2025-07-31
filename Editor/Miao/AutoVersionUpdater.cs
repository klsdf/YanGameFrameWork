using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class AutoVersionUpdater : IPreprocessBuildWithReport
{
    public int callbackOrder => -1000; // 尽早执行，确保在其他构建脚本之前

    public void OnPreprocessBuild(BuildReport report)
    {
        PlayerSettings.bundleVersion = "";
        // 备份原始版本号

        // 生成新的版本号
        string newVersion = GenerateVersionWithTimestamp(PlayerSettings.bundleVersion);

        // 更新PlayerSettings中的版本号
        PlayerSettings.bundleVersion = newVersion;

        // 同时更新平台特定的版本号
        UpdatePlatformSpecificVersion(report.summary.platform, newVersion);

        Debug.Log($"Version 打包更新，版本日期: {newVersion}");
    }

    private string GenerateVersionWithTimestamp(string baseVersion)
    {
        return $"{baseVersion}.{System.DateTime.Now:yyMMdd.HHmm}";
    }

    private void UpdatePlatformSpecificVersion(BuildTarget platform, string version)
    {
        switch (platform)
        {
            case BuildTarget.Android:
                // Android的versionCode也需要更新（必须是递增的整数）
                PlayerSettings.Android.bundleVersionCode = GenerateVersionCode();
                break;

            case BuildTarget.iOS:
                // iOS的build number
                PlayerSettings.iOS.buildNumber = System.DateTime.Now.ToString("yyyyMMddHHmm");
                break;
        }
    }

    private int GenerateVersionCode()
    {
        // 生成一个基于时间的版本代码（Android要求递增整数）
        var now = System.DateTime.Now;
        // 格式：年份后2位 + 月日时分 = 例如：2412151430
        return int.Parse($"{now:yyMMddHHmm}");
    }
}

// 运行时直接使用Application.version
public class VersionDisplay : MonoBehaviour
{
    public UnityEngine.UI.Text versionText;

    void Start()
    {
        // 现在Application.version就已经包含了构建时间！
        if (versionText != null)
        {
            versionText.text = $"v{Application.version}";
        }

        Debug.Log($"Current Version: {Application.version}");
    }

    // 如果需要解析版本号中的时间信息
    public static string GetBuildTimeFromVersion()
    {
        try
        {
            string version = Application.version;
            // 假设格式是 "1.0.0.20241215.1430"
            var parts = version.Split('.');
            if (parts.Length >= 5)
            {
                string dateStr = parts[3]; // "20241215"
                string timeStr = parts[4]; // "1430"

                if (System.DateTime.TryParseExact(dateStr + timeStr, "yyyyMMddHHmm",
                    null, System.Globalization.DateTimeStyles.None, out System.DateTime buildTime))
                {
                    return buildTime.ToString("yyyy-MM-dd HH:mm");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Failed to parse build time from version: {ex.Message}");
        }

        return "Unknown";
    }
}
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Pattern生成工具类，支持噪声、点阵、线条等多种抖动图案纹理的生成
/// </summary>
public static class PatternGenerator
{
    /// <summary>
    /// 生成Pattern对应的纹理，并赋值到Pattern对象
    /// </summary>
    /// <param name="pattern">Pattern ScriptableObject</param>
    public static void GeneratePatternTexture(Pattern pattern)
    {
        if (pattern == null) return;

        int size = Mathf.Max(2, pattern.TextureSize);
        Texture2D tex = new Texture2D(size, size, TextureFormat.RFloat, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Point;

        // 根据类型生成不同的Pattern
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float value = 0f;
                switch (pattern.Type)
                {
                    case PatternType.Noise:
                        value = Random.Range(pattern.MinimumValue, pattern.MaximumValue);
                        break;
                    case PatternType.Dots:
                        value = ((x + y) % Mathf.RoundToInt(pattern.ElementSize) == 0) ? pattern.MaximumValue : pattern.MinimumValue;
                        break;
                    case PatternType.Lines:
                        value = GetLinePatternValue(x, y, size, pattern);
                        break;
                }
                tex.SetPixel(x, y, new Color(value, value, value, 1));
            }
        }
        tex.Apply();

        pattern.Texture = tex;
        pattern.HasTexture = true;
        pattern.IsDirty = false;

        // 标记资源已修改
        EditorUtility.SetDirty(pattern);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 生成线条Pattern的像素值
    /// </summary>
    private static float GetLinePatternValue(int x, int y, int size, Pattern pattern)
    {
        float value = pattern.MinimumValue;
        int interval = Mathf.Max(1, Mathf.RoundToInt(pattern.ElementSize));
        switch (pattern.Direction)
        {
            case LineDirection.Vertical:
                if (x % interval == 0) value = pattern.MaximumValue;
                break;
            case LineDirection.Horizontal:
                if (y % interval == 0) value = pattern.MaximumValue;
                break;
            case LineDirection.Slope45:
                if ((x + y) % interval == 0) value = pattern.MaximumValue;
                break;
            case LineDirection.Slope135:
                if ((x - y + size) % interval == 0) value = pattern.MaximumValue;
                break;
        }
        return value;
    }

    /// <summary>
    /// 保存纹理为Asset
    /// </summary>
    /// <param name="tex">要保存的纹理</param>
    /// <param name="path">保存路径</param>
    public static void SaveTextureAsAsset(Texture2D tex, string path)
    {
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        Debug.Log("已保存纹理到: " + path);
    }

    public static Texture2D GeneratePatternTextureStandalone(Pattern pattern)
    {
        if (pattern == null) return null;

        int size = Mathf.Max(2, pattern.TextureSize);
        Texture2D tex = new Texture2D(size, size, TextureFormat.RFloat, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Point;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float value = 0f;
                switch (pattern.Type)
                {
                    case PatternType.Noise:
                        value = Random.Range(pattern.MinimumValue, pattern.MaximumValue);
                        break;
                    case PatternType.Dots:
                        value = ((x + y) % Mathf.RoundToInt(pattern.ElementSize) == 0) ? pattern.MaximumValue : pattern.MinimumValue;
                        break;
                    case PatternType.Lines:
                        value = GetLinePatternValue(x, y, size, pattern);
                        break;
                }
                tex.SetPixel(x, y, new Color(value, value, value, 1));
            }
        }
        tex.Apply();
        return tex;
    }
}

/// <summary>
/// Pattern生成菜单，右键Pattern资源可一键生成纹理Asset
/// </summary>
public class PatternGeneratorEditor : Editor
{
    [MenuItem("Assets/生成抖动Pattern纹理")]
    private static void GeneratePatternTextureMenu()
    {
        Pattern pattern = Selection.activeObject as Pattern;
        if (pattern != null)
        {
            // 生成纹理
            Texture2D tex = PatternGenerator.GeneratePatternTextureStandalone(pattern);

            // 选择保存路径
            string patternName = pattern.name;
            string folder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(pattern));
            string texPath = $"{folder}/Pattern_{pattern.Type}_{patternName}.asset";

            // 创建为Asset
            AssetDatabase.CreateAsset(tex, texPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Pattern纹理已生成并保存为Asset：{texPath}");
        }
    }
}

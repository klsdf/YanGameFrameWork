using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class AssetLocatorWindow : EditorWindow
{
    private const string SCRIPTS_PATH = "Assets/Scripts";
    private const string ART_PATH = "Assets/Sprites";
    private const string SCENES_PATH = "Assets/Scenes";

    private const string PREFAB_PATH = "Assets/Resources/Items";
    private const string SHADER_PATH = "Assets/Shaders";

    private const string SETTINGS_PATH = "Assets/Settings";

    [MenuItem("😁YanGameFrameWork😁/快速定位")]
    public static void ShowWindow()
    {
        GetWindow<AssetLocatorWindow>("快速定位");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        if (GUILayout.Button("定位代码目录"))
        {
            LocateFolder(SCRIPTS_PATH);
        }

        if (GUILayout.Button("定位美术目录"))
        {
            LocateFolder(ART_PATH);
        }

        if (GUILayout.Button("定位场景目录"))
        {
            LocateFolder(SCENES_PATH);
        }
         if (GUILayout.Button("定位预制体目录"))
        {
            LocateFolder(PREFAB_PATH);
        }
         if (GUILayout.Button("定位Shader目录"))
        {
            LocateFolder(SHADER_PATH);
        }
        if (GUILayout.Button("定位设置目录"))
        {
            LocateFolder(SETTINGS_PATH);
        }
    }

    private void LocateFolder(string path)
    {
        var folder = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (folder != null)
        {
            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }
        else
        {
            Debug.LogWarning($"目录不存在: {path}");
        }
    }
}
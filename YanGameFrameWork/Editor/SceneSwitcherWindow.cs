using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class SceneSwitcherWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private string searchString = "";
    private List<string> scenePaths = new List<string>();
    private List<string> filteredScenePaths = new List<string>();
    private bool showOnlyBuildScenes = false;

    [MenuItem("😁YanGameFrameWork😁/场景切换器")]
    public static void ShowWindow()
    {
        GetWindow<SceneSwitcherWindow>("场景切换器");
    }

    private void OnEnable()
    {
        RefreshSceneList();
    }

    private void RefreshSceneList()
    {
        scenePaths.Clear();

        // 获取Scenes文件夹下的所有场景文件
        string scenesFolder = "Assets/Scenes";
        if (AssetDatabase.IsValidFolder(scenesFolder))
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { scenesFolder });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                scenePaths.Add(path);
            }
        }
        else
        {
            // 如果Scenes文件夹不存在，则获取所有场景文件
            string[] guids = AssetDatabase.FindAssets("t:Scene");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                scenePaths.Add(path);
            }
        }

        // 按路径排序
        scenePaths.Sort();

        UpdateFilteredScenes();
    }

    private void UpdateFilteredScenes()
    {
        var scenes = scenePaths;

        // 如果只显示Build Settings中的场景
        if (showOnlyBuildScenes)
        {
            scenes = scenePaths.Where(path =>
                EditorBuildSettings.scenes.Any(s => s.path == path && s.enabled)).ToList();
        }

        // 应用搜索过滤
        if (string.IsNullOrEmpty(searchString))
        {
            filteredScenePaths = scenes;
        }
        else
        {
            filteredScenePaths = scenes.Where(path =>
                path.ToLower().Contains(searchString.ToLower())).ToList();
        }
    }

    private void OnGUI()
    {
        // 工具栏
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        // 搜索框
        searchString = EditorGUILayout.TextField(searchString, EditorStyles.toolbarSearchField);

        // 只显示Build Settings场景的开关
        EditorGUI.BeginChangeCheck();
        showOnlyBuildScenes = EditorGUILayout.Toggle("仅显示Build Settings场景", showOnlyBuildScenes);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateFilteredScenes();
        }

        // 刷新按钮
        if (GUILayout.Button("刷新", EditorStyles.toolbarButton, GUILayout.Width(50)))
        {
            RefreshSceneList();
        }

        EditorGUILayout.EndHorizontal();

        // 场景列表
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        foreach (string scenePath in filteredScenePaths)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            bool isInBuildSettings = EditorBuildSettings.scenes.Any(s => s.path == scenePath && s.enabled);

            // 使用不同的样式来区分是否在Build Settings中
            GUIStyle buttonStyle = isInBuildSettings ? new GUIStyle(EditorStyles.toolbarButton)
            {
                normal = { textColor = Color.white }
            } : new GUIStyle(EditorStyles.toolbarButton)
            {
                normal = { textColor = Color.gray }
            };

            if (GUILayout.Button(sceneName, buttonStyle))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
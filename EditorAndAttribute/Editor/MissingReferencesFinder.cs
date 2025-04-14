using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MissingReferencesFinder : EditorWindow
{
    private Vector2 scrollPosition;
    private List<MissingReference> missingReferences = new List<MissingReference>();
    private bool includeInactive = true;
    private bool includePrefabInstances = true;

    // 用于存储丢失引用的数据结构
    private class MissingReference
    {
        public GameObject GameObject;
        public string ComponentName;  // 改为string类型，因为Component可能完全丢失
        public string Details;
    }

    // 添加菜单项
    [MenuItem("😁YanGameFrameWork😁/寻找丢失引用")]
    public static void ShowWindow()
    {
        GetWindow<MissingReferencesFinder>("寻找丢失引用");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);

        includeInactive = EditorGUILayout.Toggle("Include Inactive Objects", includeInactive);
        includePrefabInstances = EditorGUILayout.Toggle("Include Prefab Instances", includePrefabInstances);

        if (GUILayout.Button("Scan Current Scene"))
        {
            FindMissingReferences();
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Results:", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        foreach (var reference in missingReferences)
        {
            EditorGUILayout.BeginHorizontal("box");

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                Selection.activeGameObject = reference.GameObject;
                EditorGUIUtility.PingObject(reference.GameObject);
            }

            EditorGUILayout.LabelField($"GameObject: {reference.GameObject.name}");
            EditorGUILayout.LabelField($"Issue: {reference.ComponentName}");
            EditorGUILayout.LabelField($"Details: {reference.Details}");

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void FindMissingReferences()
    {
        missingReferences.Clear();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(includeInactive);

        float total = allObjects.Length;
        float current = 0;

        foreach (GameObject obj in allObjects)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Scanning Scene",
                $"Checking {obj.name}", current / total))
            {
                break;
            }
            current++;

            // 获取所有SerializedComponents（包括丢失的脚本）
            var components = obj.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];

                // 检查丢失的脚本
                if (component == null)
                {
                    missingReferences.Add(new MissingReference
                    {
                        GameObject = obj,
                        ComponentName = "Missing Script",
                        Details = "Script cannot be loaded"
                    });
                    continue;
                }

                // 检查序列化对象中的丢失引用
                SerializedObject serializedObject = new SerializedObject(component);
                SerializedProperty property = serializedObject.GetIterator();

                while (property.NextVisible(true))
                {
                    if (property.propertyType == SerializedPropertyType.ObjectReference &&
                        property.objectReferenceValue == null &&
                        !property.hasMultipleDifferentValues &&
                        !string.IsNullOrEmpty(property.propertyPath))
                    {
                        missingReferences.Add(new MissingReference
                        {
                            GameObject = obj,
                            ComponentName = component.GetType().Name,
                            Details = $"Missing reference in field: {property.propertyPath}"
                        });
                    }
                }
            }
        }

        EditorUtility.ClearProgressBar();
        Repaint();
    }
}
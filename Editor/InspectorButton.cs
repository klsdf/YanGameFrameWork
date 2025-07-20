using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using YanGameFrameWork.Editor;

[CustomEditor(typeof(SkillSystem), true)]
[CanEditMultipleObjects]
public class InspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Debug.Log("target:" + target);
        var mono = target as SkillSystem;
        if (mono == null)
            return;

        var methods = mono.GetType()
            .GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Static
            ).Where(method =>
                Attribute.IsDefined(method, typeof(ButtonAttribute))
            ).ToArray();
        foreach (var method in methods)
        {

            var attr = method.GetCustomAttribute<ButtonAttribute>();
            DrawButton(method, attr.ButtonName);
        }
    }

    private void DrawButton(MethodInfo methodInfo, string methodName)
    {
        if (string.IsNullOrEmpty(methodName))
            methodName = methodInfo.Name;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(
            methodName,
            GUILayout.ExpandWidth(true)
        ))
        {
            foreach (var targetObj in targets)
            {
                var mono = targetObj as SkillSystem;
                if (mono == null)
                    continue;

                var val = methodInfo.Invoke(mono, new object[] { });
                if (val is IEnumerator coroutine)
                    mono.StartCoroutine(coroutine);
                else if (val != null)
                    Debug.Log($"{methodName}调用结果: {val}");
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}
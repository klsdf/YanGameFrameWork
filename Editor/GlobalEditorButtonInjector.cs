// using UnityEngine;
// using UnityEditor;
// using System.Reflection;
// using System;
// using System.Collections;
// using System.Linq;

// [InitializeOnLoad]
// public static class GlobalEditorButtonInjector
// {
//     static GlobalEditorButtonInjector()
//     {
//         // 每次 Inspector 渲染完标题后调用
//         Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
//     }

//     private static void OnPostHeaderGUI(Editor editor)
//     {
//         // 只对 MonoBehaviour 起作用
//         if (!(editor.target is MonoBehaviour mono))
//             return;

//         var targetType = mono.GetType();
//         var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
//             .Where(m => m.GetCustomAttribute<InspectorButtonAttribute>() != null);

//         foreach (var method in methods)
//         {
//             var attr = method.GetCustomAttribute<InspectorButtonAttribute>();
//             var label = string.IsNullOrEmpty(attr.Name) ? method.Name : attr.Name;
//             Debug.Log("method:" + method);

//             if (GUILayout.Button(label))
//             {
//                 var result = method.Invoke(mono, null);

//                 if (result is IEnumerator coroutine)
//                     mono.StartCoroutine(coroutine);
//                 else if (result != null)
//                     Debug.Log($"{label} 返回值: {result}");
//             }
//         }
//     }
// }

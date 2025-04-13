// using UnityEditor;
// using UnityEngine;
// using System;
// using System.Reflection;
// using System.Linq;
// using UnityEditor.Callbacks;

// [InitializeOnLoad]
// public static class InspectorButtonRegistry
// {
//     static InspectorButtonRegistry()
//     {
//         // 监听加载完成事件（或使用 [DidReloadScripts]）
//         EditorApplication.delayCall += ScanAllButtonMethods;
//     }

//     public static void ScanAllButtonMethods()
//     {
//         var allMonoTypes = TypeCache.GetTypesDerivedFrom<MonoBehaviour>();
//         foreach (var type in allMonoTypes)
//         {
//             var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

//             foreach (var method in methods)
//             {
//                 if (method.GetCustomAttribute<InspectorButtonAttribute>() != null)
//                 {
//                     // Debug.Log($"检测到按钮方法：{type.Name}.{method.Name}");
//                     // 可注册到字典中：type -> List<MethodInfo>
//                 }
//             }
//         }
//     }
// }

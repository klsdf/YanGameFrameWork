using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using YanGameFrameWork.Editor;

/// <summary>
/// 自定义编辑器基类
/// </summary>
/// <typeparam name="T">目标组件类型</typeparam>
public abstract class BaseEditor<T> : Editor where T : MonoBehaviour
{
    // 用于缓存参数输入
    private Dictionary<string, object> _paramInputs = new Dictionary<string, object>();

    public override void OnInspectorGUI()
    {
        // 绘制默认的Inspector
        DrawDefaultInspector();

        // 获取目标组件
        T targetComponent = (T)target;

        // 获取类型信息
        var methods = targetComponent.GetType().GetMethods(
            BindingFlags.Instance |
            BindingFlags.NonPublic |
            BindingFlags.Public
        );

        // 遍历所有方法找到带Button特性的
        foreach (var method in methods)
        {
            var buttonAttribute = method.GetCustomAttributes(typeof(ButtonAttribute), true)
                                      .FirstOrDefault() as ButtonAttribute;

            if (buttonAttribute != null)
            {
                string buttonName = buttonAttribute.ButtonName ?? method.Name;
                var parameters = method.GetParameters();
                object[] paramValues = new object[parameters.Length];

                // 动态生成参数输入框
                for (int i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    string key = method.Name + "_" + param.Name;

                    // 初始化默认值
                    if (!_paramInputs.ContainsKey(key))
                        _paramInputs[key] = param.HasDefaultValue ? param.DefaultValue : GetDefault(param.ParameterType);

                    // 根据类型生成输入框
                    if (param.ParameterType == typeof(string))
                        _paramInputs[key] = EditorGUILayout.TextField(param.Name, (string)_paramInputs[key]);
                    else if (param.ParameterType == typeof(int))
                        _paramInputs[key] = EditorGUILayout.IntField(param.Name, (int)_paramInputs[key]);
                    else if (param.ParameterType == typeof(float))
                        _paramInputs[key] = EditorGUILayout.FloatField(param.Name, (float)_paramInputs[key]);
                    else if (param.ParameterType == typeof(bool))
                        _paramInputs[key] = EditorGUILayout.Toggle(param.Name, (bool)_paramInputs[key]);
                    else
                        EditorGUILayout.LabelField(param.Name, $"不支持的类型: {param.ParameterType.Name}");

                    paramValues[i] = _paramInputs[key];
                }

                // 按钮
                if (GUILayout.Button(buttonName))
                {
                    method.Invoke(targetComponent, paramValues);
                }
            }
        }
    }

    // 获取类型的默认值
    private object GetDefault(System.Type t)
    {
        if (t.IsValueType) return System.Activator.CreateInstance(t);
        return null;
    }
}

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using YanGameFrameWork.Editor;

public abstract class BaseEditor<T> : Editor where T : MonoBehaviour
{
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

                // 创建按钮
                if (GUILayout.Button(buttonName))
                {
                    // 调用方法
                    method.Invoke(targetComponent, null);
                }
            }
        }
    }
}

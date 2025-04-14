// 这个文件必须放在Editor文件夹下！
using UnityEngine;
using UnityEditor;
using YanGameFrameWork.AchievementSystem;
using System.Linq;
using YanGameFrameWork.Editor;


[CustomEditor(typeof(AchievementSystem))]
public class AchievementSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制默认的Inspector
        DrawDefaultInspector();

        // 获取目标组件
        AchievementSystem achievementSystem = (AchievementSystem)target;

        // 获取类型信息
        var methods = achievementSystem.GetType().GetMethods(
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public
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
                    method.Invoke(achievementSystem, null);
                }
            }
        }
    }
}
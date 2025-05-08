using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YanGameFrameWork.TutoriaSystem;
[CustomEditor(typeof(MaskedUI))]
public class MaskedUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 获取目标对象
        MaskedUI maskedUI = (MaskedUI)target;

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(maskedUI), typeof(MonoScript), false);
        GUI.enabled = true;

        // 开始检查属性的变化
        serializedObject.Update();

        // 绘制父类的color属性
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Color"), new GUIContent("Color"));

        // 绘制你希望显示的属性
        EditorGUILayout.PropertyField(serializedObject.FindProperty("paddingTop"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("paddingBottom"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("paddingLeft"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("paddingRight"));

#if USE_LIBTESSDOTNET
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_targets"));
#else
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_target"));
#endif


        if (GUILayout.Button("测试高亮"))
        {
            // 调用摄像机震动方法，设置持续时间和幅度
            maskedUI.HighlightTarget();
        }
        // 应用属性的变化
        serializedObject.ApplyModifiedProperties();
    }
}

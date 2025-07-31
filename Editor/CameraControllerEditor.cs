using UnityEditor;
using UnityEngine;
using YanGameFrameWork.CameraController;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    // 定义 SerializedProperty 来存储震动参数
    private float _shakeDurationProp = 0.2f;
    private float _shakeMagnitudeProp = 0.15f;



    public override void OnInspectorGUI()
    {
        CameraController cameraController = (CameraController)target;
        // 显示脚本部分为只读
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(cameraController), typeof(MonoScript), false);
        GUI.enabled = true;

        // 其他字段保持可编辑
        cameraController.controlCamera = (Camera)EditorGUILayout.ObjectField("控制摄像机", cameraController.controlCamera, typeof(Camera), true);

        // 绘制默认的 Inspector
        // DrawDefaultInspector();
        cameraController.IsEnableDarg = EditorGUILayout.Toggle("是否启用拖拽", cameraController.IsEnableDarg);

        // 根据 isEnableDarg 显示拖拽相关参数
        if (cameraController.IsEnableDarg)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("拖拽参数", EditorStyles.boldLabel);

            // 添加鼠标按键选择
            EditorGUILayout.LabelField("拖动控制按键", EditorStyles.boldLabel);
            cameraController.useLeftMouseButton = EditorGUILayout.Toggle("使用左键拖动", cameraController.useLeftMouseButton);
            cameraController.useRightMouseButton = EditorGUILayout.Toggle("使用右键拖动", cameraController.useRightMouseButton);

            // 如果没有选择任何按键，显示警告
            if (!cameraController.useLeftMouseButton && !cameraController.useRightMouseButton)
            {
                EditorGUILayout.HelpBox("警告：没有选择任何拖动按键，相机将无法拖动！", MessageType.Warning);
            }

            cameraController.dragSpeed = EditorGUILayout.FloatField("拖拽速度", cameraController.dragSpeed);
            cameraController.minX = EditorGUILayout.FloatField("最小X", cameraController.minX);
            cameraController.maxX = EditorGUILayout.FloatField("最大X", cameraController.maxX);
            cameraController.minY = EditorGUILayout.FloatField("最小Y", cameraController.minY);
            cameraController.maxY = EditorGUILayout.FloatField("最大Y", cameraController.maxY);
            EditorGUI.indentLevel--;
        }

        cameraController.IsEnableZoom = EditorGUILayout.Toggle("是否启用缩放", cameraController.IsEnableZoom);

        // 根据 isEnableZoom 显示缩放相关参数
        if (cameraController.IsEnableZoom)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("缩放参数", EditorStyles.boldLabel);
            cameraController.zoomSpeed = EditorGUILayout.FloatField("缩放速度", cameraController.zoomSpeed);
            cameraController.minFov = EditorGUILayout.FloatField("最小视野", cameraController.minFov);
            cameraController.maxFov = EditorGUILayout.FloatField("最大视野", cameraController.maxFov);
            EditorGUI.indentLevel--;
        }

        cameraController.IsEnableFollow = EditorGUILayout.Toggle("是否启用跟随", cameraController.IsEnableFollow);

        // 根据 IsEnableFollow 显示跟随相关参数
        if (cameraController.IsEnableFollow)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("跟随参数", EditorStyles.boldLabel);
            cameraController.followTarget = (Transform)EditorGUILayout.ObjectField("跟随目标", cameraController.followTarget, typeof(Transform), true);
            cameraController.moveFactor = EditorGUILayout.FloatField("移动比例", cameraController.moveFactor);
            EditorGUI.indentLevel--;
        }

        cameraController.IsEnableLookAt = EditorGUILayout.Toggle("是否启用注视", cameraController.IsEnableLookAt);
        // 根据 IsEnableLookAt 显示注视相关参数
        if (cameraController.IsEnableLookAt)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("注视参数", EditorStyles.boldLabel);
            cameraController.lookAtTarget = (Transform)EditorGUILayout.ObjectField("注视目标", cameraController.lookAtTarget, typeof(Transform), true);
            EditorGUI.indentLevel--;
        }



        // 为调试参数和函数添加背景
        GUIStyle debugStyle = new GUIStyle(GUI.skin.box);
        debugStyle.padding = new RectOffset(10, 10, 10, 10);
        debugStyle.margin = new RectOffset(10, 10, 10, 10);

        // 创建一个纹理并设置颜色
        Texture2D backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, new Color(0.9f, 0.9f, 0.9f)); // 设置为浅灰色
        backgroundTexture.Apply();

        // 设置背景颜色
        debugStyle.normal.background = backgroundTexture;

        EditorGUILayout.BeginVertical(debugStyle);
        EditorGUILayout.LabelField("调试区域", EditorStyles.boldLabel);

        _shakeDurationProp = EditorGUILayout.FloatField("震动持续时间", _shakeDurationProp);
        _shakeMagnitudeProp = EditorGUILayout.FloatField("震动幅度", _shakeMagnitudeProp);

        // 添加震动测试按钮
        if (GUILayout.Button("测试摄像机震动"))
        {
            // 调用摄像机震动方法，设置持续时间和幅度
            cameraController.ShakeCamera(_shakeDurationProp, _shakeMagnitudeProp);
        }
        EditorGUILayout.EndVertical();

        // 保存更改
        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraController);
        }
    }
}

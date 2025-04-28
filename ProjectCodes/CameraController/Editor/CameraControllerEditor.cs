using UnityEditor;
using UnityEngine;
using YanGameFrameWork.CameraController;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {


        CameraController cameraController = (CameraController)target;

        cameraController.controlCamera = (Camera)EditorGUILayout.ObjectField("控制摄像机", cameraController.controlCamera, typeof(Camera), true);

        // 绘制默认的 Inspector
        // DrawDefaultInspector();
        cameraController.IsEnableDarg = EditorGUILayout.Toggle("是否启用拖拽", cameraController.IsEnableDarg);

        // 根据 isEnableDarg 显示拖拽相关参数
        if (cameraController.IsEnableDarg)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("拖拽参数", EditorStyles.boldLabel);
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


        // 保存更改
        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraController);
        }
    }
}

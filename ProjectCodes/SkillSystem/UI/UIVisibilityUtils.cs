using UnityEngine;
using UnityEngine.UI;

public static class UIVisibilityUtils
{
    /// <summary>
    /// 检测UI元素是否在屏幕可见范围内
    /// </summary>
    /// <param name="uiElement">要检测的UI元素</param>
    /// <param name="camera">使用的摄像机(默认为主摄像机)</param>
    /// <param name="partialOK">是否接受部分可见</param>
    /// <returns>是否可见</returns>
    public static bool IsUIVisible(RectTransform uiElement, Camera camera = null, bool partialOK = true)
    {
        if (uiElement == null)
        {
            Debug.LogWarning("UI元素不能为null");
            return false;
        }

        Camera cam = camera ?? Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("没有找到可用的摄像机");
            return false;
        }

        Vector3[] corners = new Vector3[4];
        uiElement.GetWorldCorners(corners);

        return partialOK ? IsPartiallyVisible(corners, cam) : IsFullyVisible(corners, cam);
    }

    /// <summary>
    /// 检测世界空间中的点是否在屏幕内
    /// </summary>
    /// <param name="worldPoint">世界坐标点</param>
    /// <param name="camera">使用的摄像机(默认为主摄像机)</param>
    /// <param name="margin">屏幕边缘余量(0-1)</param>
    /// <returns>是否在屏幕内</returns>
    public static bool IsPointOnScreen(Vector3 worldPoint, Camera camera = null, float margin = 0f)
    {
        Camera cam = camera ?? Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("没有找到可用的摄像机");
            return false;
        }

        Vector3 viewportPoint = cam.WorldToViewportPoint(worldPoint);

        // 检查是否在视口范围内(考虑余量)
        bool onScreen = viewportPoint.z > 0 &&
                       viewportPoint.x >= margin && viewportPoint.x <= 1 - margin &&
                       viewportPoint.y >= margin && viewportPoint.y <= 1 - margin;

        return onScreen;
    }

    /// <summary>
    /// 获取UI元素在屏幕上的可见比例(0-1)
    /// </summary>
    public static float GetUIVisibleRatio(RectTransform uiElement, Camera camera = null)
    {
        if (uiElement == null) return 0;

        Camera cam = camera ?? Camera.main;
        if (cam == null) return 0;

        Vector3[] corners = new Vector3[4];
        uiElement.GetWorldCorners(corners);

        float totalArea = Mathf.Abs(Vector3.Cross(corners[1] - corners[0], corners[3] - corners[0]).magnitude) / 2f;

        int visibleCorners = 0;
        foreach (Vector3 corner in corners)
        {
            if (IsPointOnScreen(corner, cam, 0f))
            {
                visibleCorners++;
            }
        }

        return Mathf.Clamp01(visibleCorners / 4f);
    }

    // --- 私有方法 ---
    private static bool IsFullyVisible(Vector3[] corners, Camera camera)
    {
        foreach (Vector3 corner in corners)
        {
            if (!IsPointOnScreen(corner, camera, 0f))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsPartiallyVisible(Vector3[] corners, Camera camera)
    {
        // 检查是否有任意角在屏幕内
        foreach (Vector3 corner in corners)
        {
            if (IsPointOnScreen(corner, camera, 0f))
            {
                return true;
            }
        }

        // 检查是否跨越屏幕边界
        bool xOverlap = false;
        bool yOverlap = false;

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Vector3 corner in corners)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(corner);
            minX = Mathf.Min(minX, viewportPoint.x);
            maxX = Mathf.Max(maxX, viewportPoint.x);
            minY = Mathf.Min(minY, viewportPoint.y);
            maxY = Mathf.Max(maxY, viewportPoint.y);
        }

        xOverlap = maxX > 0 && minX < 1;
        yOverlap = maxY > 0 && minY < 1;

        return xOverlap && yOverlap;
    }
}
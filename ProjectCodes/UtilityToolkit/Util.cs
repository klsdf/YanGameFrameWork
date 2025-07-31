using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 作者：闫辰祥
/// </summary>
public class Util
{



    /// <summary>
    /// 百分之probability的概率触发callback
    /// </summary>
    /// <param name="probability">概率</param>
    /// <param name="callback">回调方法</param>
    /// <returns>触发成功返回true，否则false</returns>
    public static bool TryTrigger(float probability, Action callback)
    {

        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll <= probability)
        {
            callback();
            return true;
        }

        return false;
    }

    public static bool TryTrigger(float probability)
    {

        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll <= probability)
        {
            return true;
        }

        return false;
    }




    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }



    public static Vector3 GetMouseWorldPosition2D()
    {
        Vector3 result = GetMouseWorldPosition();
        result.z = 0;
        return result;
    }



    /// <summary>
    /// 检测鼠标是否悬停在指定的UI对象上
    /// </summary>
    /// <param name="targetObject">要检测的UI对象</param>
    /// <returns>如果鼠标在指定UI对象上返回true，否则返回false</returns>
    public static bool IsPointerOverUI(GameObject targetObject)
    {
        if (EventSystem.current == null || targetObject == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == targetObject || result.gameObject.transform.IsChildOf(targetObject.transform))
            {
                return true;
            }
        }

        return false;
    }

}

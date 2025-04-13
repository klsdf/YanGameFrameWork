/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-02-14
 * Description: 范围选择器接口，用于判断一个物体是否在另一个物体的范围内
 *
 ****************************************************************************/
using UnityEngine;
using System;
public enum RangePosition
{
    Center,
    Left,
    Right,
    Top,
    Bottom
}

public enum RangeShapeType
{
    Circle,
    Rectangle
}



// 定义接口 IRangeShape
public interface IRangeShape
{
    bool IsInRange(Vector2 position, Vector2 targetPosition, Vector2 offset, float scale);

    IRangeShape Clone();
}

// 实现圆形范围类
public class CircleRange : IRangeShape

{
    public float radius;
    public CircleRange(float radius)
    {
        this.radius = radius;
    }
    public bool IsInRange(Vector2 position, Vector2 targetPosition, Vector2 offset, float scale)
    {
        var trueRadius = radius * scale;
        return Vector2.Distance(position + offset, targetPosition) <= trueRadius;
    }

    public IRangeShape Clone()
    {
        return new CircleRange(radius);
    }

}

// 实现矩形范围类
public class RectangleRange : IRangeShape
{
    public float width;
    public float height;
    public RectangleRange(float width, float height)
    {
        this.width = width;
        this.height = height;
    }
    public bool IsInRange(Vector2 position, Vector2 targetPosition, Vector2 offset, float scale)
    {
        var trueWidth = width * scale;
        var trueHeight = height * scale;
        Vector2 adjustedTargetPosition = targetPosition + offset;


        bool isBiggerThanLeft = adjustedTargetPosition.x >= position.x - trueWidth / 2;
        bool isSmallerThanRight = adjustedTargetPosition.x <= position.x + trueWidth / 2;
        bool isBiggerThanBottom = adjustedTargetPosition.y >= position.y - trueHeight / 2;
        bool isSmallerThanTop = adjustedTargetPosition.y <= position.y + trueHeight / 2;


        return isBiggerThanLeft && isSmallerThanRight && isBiggerThanBottom && isSmallerThanTop;
    }

    public IRangeShape Clone()
    {
        return new RectangleRange(width, height);
    }
}



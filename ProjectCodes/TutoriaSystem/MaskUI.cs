using UnityEngine;
using UnityEngine.UI;

public class MaskedUI : Graphic, ICanvasRaycastFilter
{
    [SerializeField]
    private RectTransform _target;  // 镂空区域的 RectTransform
    private Vector3 _targetBoundsMin;
    private Vector3 _targetBoundsMax;
    public float paddingTop = 10f;
    public float paddingBottom = 10f;
    public float paddingLeft = 10f;
    public float paddingRight = 10f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // 获取镂空区域的包围盒坐标，并应用四个方向的 padding
        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(this.rectTransform, this._target);
        _targetBoundsMin = bounds.min - new Vector3(paddingLeft, paddingBottom);
        _targetBoundsMax = bounds.max + new Vector3(paddingRight, paddingTop);

        if (_targetBoundsMin == Vector3.zero && _targetBoundsMax == Vector3.zero)
        {
            base.OnPopulateMesh(vh);
            return;
        }

        vh.Clear();

        Vector2 pivot = this.rectTransform.pivot;
        Rect rect = this.rectTransform.rect;
        float outerLeftBottomX = -pivot.x * rect.width;
        float outerLeftBottomY = -pivot.y * rect.height;
        float outerRightTopX = (1 - pivot.x) * rect.width;
        float outerRightTopY = (1 - pivot.y) * rect.height;

        // 准备顶点数据
        UIVertex vert = UIVertex.simpleVert;
        vert.color = this.color;

        // 计算遮罩区域顶点位置
        vert.position = new Vector3(outerLeftBottomX, outerRightTopY);
        vh.AddVert(vert); // 0 outer LeftTop
        vert.position = new Vector3(outerRightTopX, outerRightTopY);
        vh.AddVert(vert); // 1 outer RightTop
        vert.position = new Vector3(outerRightTopX, outerLeftBottomY);
        vh.AddVert(vert); // 2 outer RightBottom
        vert.position = new Vector3(outerLeftBottomX, outerLeftBottomY);
        vh.AddVert(vert); // 3 outer LeftBottom

        // 计算镂空区域顶点位置
        vert.position = new Vector3(_targetBoundsMin.x, _targetBoundsMax.y);
        vh.AddVert(vert); // 4 inner LeftTop
        vert.position = new Vector3(_targetBoundsMax.x, _targetBoundsMax.y);
        vh.AddVert(vert); // 5 inner RightTop
        vert.position = new Vector3(_targetBoundsMax.x, _targetBoundsMin.y);
        vh.AddVert(vert); // 6 inner RightBottom
        vert.position = new Vector3(_targetBoundsMin.x, _targetBoundsMin.y);
        vh.AddVert(vert); // 7 inner LeftBottom

        // 向缓冲区中添加三角形，填充遮罩区域外的部分
        vh.AddTriangle(4, 0, 1);
        vh.AddTriangle(4, 1, 5);
        vh.AddTriangle(5, 1, 2);
        vh.AddTriangle(5, 2, 6);
        vh.AddTriangle(6, 2, 3);
        vh.AddTriangle(6, 3, 7);
        vh.AddTriangle(7, 3, 0);
        vh.AddTriangle(7, 0, 4);
    }



    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        // 透传镂空区域事件
        return !RectTransformUtility.RectangleContainsScreenPoint(this._target, sp, eventCamera);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键点击
        {
            Vector2 localMousePosition = RectTransformUtility.WorldToScreenPoint(Camera.main, Input.mousePosition);
            if (IsRaycastLocationValid(localMousePosition, Camera.main))
            {
                Debug.Log("点击到了目标区域");
            }
        }
    }


    public void SetTarget(RectTransform target)
    {
        _target = target;
    }
}

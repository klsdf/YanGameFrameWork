/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-08
 *
 * Description: 用于实现UI的镂空效果，以便实现新手引导。可以支持多个目标的镂空。以及高亮镂空区域。还可以支持SpriteRenderer和UI对象的镂空。

 * 注意：
 * 1. 本系统依赖于LibTessDotNet.dll，请确保使用Nuget安装LibTessDotNet.1.1.15。
 * 2. 如果不安装，本系统会使用条件编译采用默认的逻辑，但是不支持多个目标镂空
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


#if USE_LIBTESSDOTNET
using LibTessDotNet;
#endif


namespace YanGameFrameWork.TutoriaSystem
{

    public class MaskedUI : Graphic, ICanvasRaycastFilter
    {
        public float paddingTop = 10f;
        public float paddingBottom = 10f;
        public float paddingLeft = 10f;
        public float paddingRight = 10f;

#if USE_LIBTESSDOTNET
        [SerializeField]
        private List<Transform> _targets = new List<Transform>();  // 多个目标


        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();


            // Tess 初始化
            Tess tess = new Tess();

            Vector2 pivot = this.rectTransform.pivot;
            Rect rect = this.rectTransform.rect;
            float outerLeft = -pivot.x * rect.width;
            float outerBottom = -pivot.y * rect.height;
            float outerRight = (1 - pivot.x) * rect.width;
            float outerTop = (1 - pivot.y) * rect.height;

            //添加全屏幕的遮罩（顺时针）
            tess.AddContour(new ContourVertex[]
            {
            new ContourVertex(new Vec3(outerLeft, outerBottom, 0)),
            new ContourVertex(new Vec3(outerRight, outerBottom, 0)),
            new ContourVertex(new Vec3(outerRight, outerTop, 0)),
            new ContourVertex(new Vec3(outerLeft, outerTop, 0))
            }, ContourOrientation.Clockwise);

            // 挖洞（逆时针）
            foreach (var target in _targets)
            {
                if (target == null)
                    continue;

                // 判断是否为UI元素
                if (target is RectTransform)
                {
                    // 处理UI元素
                    Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(this.rectTransform, target as RectTransform);
                    float holeLeft = bounds.min.x - paddingLeft;
                    float holeRight = bounds.max.x + paddingRight;
                    float holeBottom = bounds.min.y - paddingBottom;
                    float holeTop = bounds.max.y + paddingTop;

                    print($"rect的值：holeLeft: {holeLeft}, holeRight: {holeRight}, holeBottom: {holeBottom}, holeTop: {holeTop}");

                    tess.AddContour(new ContourVertex[]
                    {
                    new ContourVertex(new Vec3(holeLeft, holeTop, 0)),
                    new ContourVertex(new Vec3(holeRight, holeTop, 0)),
                    new ContourVertex(new Vec3(holeRight, holeBottom, 0)),
                    new ContourVertex(new Vec3(holeLeft, holeBottom, 0))
                    }, ContourOrientation.CounterClockwise);
                }
                else
                {
                    // 使用SpriteRenderer来计算边界
                    SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        Vector3 min = spriteRenderer.bounds.min;
                        Vector3 max = spriteRenderer.bounds.max;

                        Vector3[] worldCorners = new Vector3[]
                        {
                        new Vector3(min.x, min.y, min.z),
                        new Vector3(max.x, min.y, min.z),
                        new Vector3(max.x, max.y, min.z),
                        new Vector3(min.x, max.y, min.z),
                        };

                        Vector2[] uiCorners = new Vector2[4];
                        //如果 UI 是 Overlay 模式，RectTransformUtility 需要传 null 做相机；
                        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldCorners[i]);
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, screenPoint, cam, out uiCorners[i]);
                        }

                        float holeLeft = Mathf.Min(uiCorners[0].x, uiCorners[1].x, uiCorners[2].x, uiCorners[3].x) - paddingLeft;
                        float holeRight = Mathf.Max(uiCorners[0].x, uiCorners[1].x, uiCorners[2].x, uiCorners[3].x) + paddingRight;
                        float holeBottom = Mathf.Min(uiCorners[0].y, uiCorners[1].y, uiCorners[2].y, uiCorners[3].y) - paddingBottom;
                        float holeTop = Mathf.Max(uiCorners[0].y, uiCorners[1].y, uiCorners[2].y, uiCorners[3].y) + paddingTop;

                        tess.AddContour(new ContourVertex[]
                        {
                        new ContourVertex(new Vec3(holeLeft, holeTop, 0)),
                        new ContourVertex(new Vec3(holeRight, holeTop, 0)),
                        new ContourVertex(new Vec3(holeRight, holeBottom, 0)),
                        new ContourVertex(new Vec3(holeLeft, holeBottom, 0))
                        }, ContourOrientation.CounterClockwise);
                    }
                }
            }

            // 执行剖分
            tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

            // 绘制三角形
            for (int i = 0; i < tess.ElementCount; i++)
            {
                int idx0 = tess.Elements[i * 3];
                int idx1 = tess.Elements[i * 3 + 1];
                int idx2 = tess.Elements[i * 3 + 2];

                // 忽略无效三角形
                if (idx0 == -1 || idx1 == -1 || idx2 == -1)
                    continue;

                var v0 = tess.Vertices[idx0].Position;
                var v1 = tess.Vertices[idx1].Position;
                var v2 = tess.Vertices[idx2].Position;

                AddTriangle(vh, new Vector2(v0.X, v0.Y), new Vector2(v1.X, v1.Y), new Vector2(v2.X, v2.Y));
            }

        }

        private void AddTriangle(VertexHelper vh, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            int idx = vh.currentVertCount;

            UIVertex vert = UIVertex.simpleVert;
            vert.color = this.color;

            vert.position = p0;
            vh.AddVert(vert);
            vert.position = p1;
            vh.AddVert(vert);
            vert.position = p2;
            vh.AddVert(vert);

            vh.AddTriangle(idx, idx + 1, idx + 2);
        }
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            foreach (var target in _targets)
            {
                if (target != null && RectTransformUtility.RectangleContainsScreenPoint(target as RectTransform, sp, eventCamera))
                {
                    return false; // 命中任意 target 就透传
                }
            }
            return true;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 localMousePosition = RectTransformUtility.WorldToScreenPoint(Camera.main, Input.mousePosition);
                foreach (var target in _targets)
                {
                    if (target != null && RectTransformUtility.RectangleContainsScreenPoint(target as RectTransform, localMousePosition, Camera.main))
                    {
                        Debug.Log("点击到了目标区域");
                    }
                }
            }
        }

        public void SetTargets(List<Transform> targets)
        {
            _targets = targets;
        }

        public void AddTarget(Transform target)
        {
            if (!_targets.Contains(target))
            {
                _targets.Add(target);
            }
        }

        public void ClearTargets()
        {
            _targets.Clear();
        }



        public void HighlightTargets()
        {
            StartCoroutine(AnimateHighlight());
        }

        private IEnumerator AnimateHighlight()
        {
            float initialPadding = 100f; // 初始较大的padding
            float duration = 0.5f; // 动画持续时间
            float elapsedTime = 0f;

            float startPaddingTop = initialPadding;
            float startPaddingBottom = initialPadding;
            float startPaddingLeft = initialPadding;
            float startPaddingRight = initialPadding;

            float endPaddingTop = paddingTop;
            float endPaddingBottom = paddingBottom;
            float endPaddingLeft = paddingLeft;
            float endPaddingRight = paddingRight;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                paddingTop = Mathf.Lerp(startPaddingTop, endPaddingTop, t);
                paddingBottom = Mathf.Lerp(startPaddingBottom, endPaddingBottom, t);
                paddingLeft = Mathf.Lerp(startPaddingLeft, endPaddingLeft, t);
                paddingRight = Mathf.Lerp(startPaddingRight, endPaddingRight, t);

                SetVerticesDirty(); // 更新UI

                yield return null;
            }

            // 确保最终值为目标值
            paddingTop = endPaddingTop;
            paddingBottom = endPaddingBottom;
            paddingLeft = endPaddingLeft;
            paddingRight = endPaddingRight;

            SetVerticesDirty();
        }
#else
    [SerializeField]
    private RectTransform _target;  // 镂空区域的 RectTransform

    private Vector3 _targetBoundsMin;
    private Vector3 _targetBoundsMax;
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



#endif





    }




}
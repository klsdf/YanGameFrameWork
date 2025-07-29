using UnityEngine;

namespace CardDraw.UI
{
    /// <summary>
    /// 使用LineRenderer绘制圆环的组件
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class LineCircle : MonoBehaviour
    {
        [Header("圆环设置")]
        [SerializeField] private float radius = 1f;           // 圆环半径
        [SerializeField] private int segments = 64;           // 圆环分段数
        [SerializeField] private float lineWidth = 0.1f;      // 线条宽度
        [SerializeField] private Material lineMaterial;       // 线条材质

        private float diameter => radius * 2f;
        
        [Header("线段的数量")]
        [SerializeField] private Vector2 textureTiling = new Vector2(8.85f, 1f);  // 纹理缩放
        
        [Header("颜色设置")]
        [SerializeField] private Color lineColor = Color.white; // 线条颜色
        
        [Header("动画设置")]
        [SerializeField] private bool enableAnimation = false; // 是否启用动画
        [SerializeField] private float animationSpeed = 1f;    // 动画速度`

        
        private LineRenderer lineRenderer;
        private Vector3[] circlePoints;
        private float currentAngle = 0f;
        
        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            InitializeLineRenderer();
            GenerateCircle();
        }
        
        private void Start()
        {
            UpdateCircle();
        }
        
        private void Update()
        {
            if (enableAnimation)
            {
                UpdateAnimation();
            }
        }
        
        /// <summary>
        /// 初始化LineRenderer组件
        /// </summary>
        private void InitializeLineRenderer()
        {
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            
            if (lineMaterial != null)
            {
                lineRenderer.material = lineMaterial;
                // 设置纹理的tiling和offset
                lineRenderer.sharedMaterial.mainTextureScale = textureTiling;
                
                // 设置shader属性
                if (lineRenderer.sharedMaterial.HasProperty("_Color"))
                {
                    lineRenderer.sharedMaterial.SetColor("_Color", lineColor);
                }

                if (lineRenderer.sharedMaterial.HasProperty("_Tiling"))
                {
                    lineRenderer.sharedMaterial.SetVector("_Tiling", new Vector4(textureTiling.x, textureTiling.y, 0, 0));
                }
            }
        }
        
        /// <summary>
        /// 生成圆环的点
        /// </summary>
        private void GenerateCircle()
        {
            circlePoints = new Vector3[segments + 1];
            
            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)i / segments * 2f * Mathf.PI;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                circlePoints[i] = new Vector3(x, y, 0f);
            }
        }
        
        /// <summary>
        /// 更新圆环显示
        /// </summary>
        private void UpdateCircle()
        {
            lineRenderer.positionCount = circlePoints.Length;
            lineRenderer.SetPositions(circlePoints);
        }
        
        /// <summary>
        /// 更新动画
        /// </summary>
        private void UpdateAnimation()
        {
            currentAngle += animationSpeed * Time.deltaTime;
            
            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)i / segments * 2f * Mathf.PI + currentAngle ;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                circlePoints[i] = new Vector3(x, y, 0f);
            }
            
            UpdateCircle();
        }
        
        /// <summary>
        /// 设置圆环半径
        /// </summary>
        /// <param name="newRadius">新的半径值</param>
        public void SetRadius(float newRadius)
        {
            radius = newRadius;
            GenerateCircle();
            UpdateCircle();
        }
        
        /// <summary>
        /// 设置线条宽度
        /// </summary>
        /// <param name="width">新的线条宽度</param>
        public void SetLineWidth(float width)
        {
            lineWidth = width;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }
        
        /// <summary>
        /// 设置线条颜色
        /// </summary>
        /// <param name="color">新的颜色</param>
        public void SetLineColor(Color color)
        {
            lineColor = color;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            
            // 更新shader中的颜色
            if (lineRenderer.sharedMaterial != null && lineRenderer.sharedMaterial.HasProperty("_Color"))
            {
                lineRenderer.sharedMaterial.SetColor("_Color", lineColor);
            }
        }
        
   
        
        /// <summary>
        /// 设置纹理缩放
        /// </summary>
        /// <param name="tiling">纹理缩放值</param>
        public void SetTextureTiling(Vector2 tiling)
        {
            textureTiling = tiling;
            if (lineRenderer.sharedMaterial != null)
            {
                lineRenderer.sharedMaterial.mainTextureScale = textureTiling;
            }
        }
        
   
        
        /// <summary>
        /// 设置动画状态
        /// </summary>
        /// <param name="enabled">是否启用动画</param>
        public void SetAnimationEnabled(bool enabled)
        {
            enableAnimation = enabled;
        }
        
        /// <summary>
        /// 设置动画速度
        /// </summary>
        /// <param name="speed">动画速度</param>
        public void SetAnimationSpeed(float speed)
        {
            animationSpeed = speed;
        }
        
        /// <summary>
        /// 获取当前半径
        /// </summary>
        /// <returns>当前半径值</returns>
        public float GetRadius()
        {
            return radius;
        }
        
        /// <summary>
        /// 获取当前线条宽度
        /// </summary>
        /// <returns>当前线条宽度</returns>
        public float GetLineWidth()
        {
            return lineWidth;
        }
        
        /// <summary>
        /// 获取当前线条颜色
        /// </summary>
        /// <returns>当前线条颜色</returns>
        public Color GetLineColor()
        {
            return lineColor;
        }
        
        /// <summary>
        /// 获取当前纹理缩放
        /// </summary>
        /// <returns>当前纹理缩放</returns>
        public Vector2 GetTextureTiling()
        {
            return textureTiling;
        }
        

        
        /// <summary>
        /// 获取动画是否启用
        /// </summary>
        /// <returns>动画是否启用</returns>
        public bool IsAnimationEnabled()
        {
            return enableAnimation;
        }
        
        /// <summary>
        /// 获取动画速度
        /// </summary>
        /// <returns>当前动画速度</returns>
        public float GetAnimationSpeed()
        {
            return animationSpeed;
        }
        
        /// <summary>
        /// 当Inspector中的值发生变化时调用
        /// </summary>
        private void OnValidate()
        {
            // 确保LineRenderer组件存在
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
            }
            
            // 初始化LineRenderer（如果还没初始化）
            if (lineRenderer != null)
            {
                InitializeLineRenderer();
            }
            
            // 重新生成圆环并更新显示
            GenerateCircle();
            UpdateCircle();
            
            // 更新线条宽度和颜色
            if (lineRenderer != null)
            {
                SetLineWidth(lineWidth);
                SetLineColor(lineColor);
            }
        }
    }
}

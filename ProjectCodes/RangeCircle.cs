using UnityEngine;

namespace CardDraw.UI
{

    [RequireComponent(typeof(SpriteRenderer))]
    /// <summary>
    /// 可控制大小和颜色的世界空间圆形精灵
    /// </summary>
    public class RangeCircle : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("圆形设置")]
        [SerializeField] private float radius = 1f;

        private float diameter => radius * 2f;
        [SerializeField] private Color circleColor = Color.white;
        [SerializeField] private bool useCircleSprite = true;
        

        /// <summary>
        /// 圆形颜色
        /// </summary>
        public Color CircleColor
        {
            get => circleColor;
            set
            {
                circleColor = value;
                UpdateCircleColor();
            }
        }
        
    
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            SetupCircle();
        }
        
        /// <summary>
        /// 当Inspector中的值发生变化时调用
        /// </summary>
        private void OnValidate()
        {
            // 确保组件已初始化
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            // 在编辑模式下也创建精灵
            if (!Application.isPlaying && useCircleSprite )
            {
                spriteRenderer.sprite = CreateCircleSprite();
            }
            
            // 更新圆形大小和颜色
            
            UpdateCircleSize();
            UpdateCircleColor();
        }
        
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponents()
        {
            // 获取或创建SpriteRenderer组件
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }
        }
        
        /// <summary>
        /// 设置圆形
        /// </summary>
        private void SetupCircle()
        {
            if (useCircleSprite)
            {
                // 使用Unity内置的圆形精灵
                spriteRenderer.sprite = CreateCircleSprite();
            }
            
            UpdateCircleSize();
            UpdateCircleColor();
        }
        
        /// <summary>
        /// 创建圆形精灵
        /// </summary>
        /// <returns>圆形精灵</returns>
        private Sprite CreateCircleSprite()
        {
            // 根据半径计算纹理大小，确保纹理分辨率足够
            // int textureSize = Mathf.Max(64, Mathf.RoundToInt(radius * 32f));
            int textureSize = 400;
            Texture2D texture = new Texture2D(textureSize, textureSize);
            
            Vector2 center = new Vector2(textureSize / 2f, textureSize / 2f);
            float textureRadius = textureSize / 2f - 1f;
            
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    Vector2 point = new Vector2(x, y);
                    float distance = Vector2.Distance(point, center);
                    
                    if (distance <= textureRadius)
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
            
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
        }
        
        /// <summary>
        /// 更新圆形大小
        /// </summary>
        private void UpdateCircleSize()
        {
            if (spriteRenderer != null)
            {
                // 由于Sprite的pivot在中心，默认大小为1，所以直接使用半径值
                // 这样与LineRenderer的半径值保持一致
                transform.localScale = new Vector3(radius, radius, 1f);
            }
        }
        
        /// <summary>
        /// 更新圆形颜色
        /// </summary>
        private void UpdateCircleColor()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = circleColor;
            }
        }
        
   
        
        /// <summary>
        /// 设置圆形颜色
        /// </summary>
        /// <param name="newColor">新的颜色</param>
        public void SetColor(Color newColor)
        {
            CircleColor = newColor;
        }
        
        /// <summary>
        /// 设置圆形颜色（RGB值）
        /// </summary>
        /// <param name="r">红色分量 (0-1)</param>
        /// <param name="g">绿色分量 (0-1)</param>
        /// <param name="b">蓝色分量 (0-1)</param>
        /// <param name="a">透明度 (0-1)</param>
        public void SetColor(float r, float g, float b, float a = 1f)
        {
            SetColor(new Color(r, g, b, a));
        }
        
     
        
        /// <summary>
        /// 淡入动画
        /// </summary>
        /// <param name="duration">动画持续时间</param>
        public void FadeIn(float duration = 0.5f)
        {
            StartCoroutine(FadeCoroutine(0f, 1f, duration));
        }
        
        /// <summary>
        /// 淡出动画
        /// </summary>
        /// <param name="duration">动画持续时间</param>
        public void FadeOut(float duration = 0.5f)
        {
            StartCoroutine(FadeCoroutine(1f, 0f, duration));
        }
        
        /// <summary>
        /// 淡入淡出协程
        /// </summary>
        private System.Collections.IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration)
        {
            float elapsed = 0f;
            Color startColor = spriteRenderer.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, endAlpha);
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                spriteRenderer.color = Color.Lerp(startColor, endColor, t);
                yield return null;
            }
            
            spriteRenderer.color = endColor;
        }
        
        /// <summary>
        /// 缩放动画
        /// </summary>
        /// <param name="targetScale">目标缩放</param>
        /// <param name="duration">动画持续时间</param>
        public void ScaleTo(float targetScale, float duration = 0.5f)
        {
            StartCoroutine(ScaleCoroutine(transform.localScale, Vector3.one * targetScale, duration));
        }
        
        /// <summary>
        /// 缩放协程
        /// </summary>
        private System.Collections.IEnumerator ScaleCoroutine(Vector3 startScale, Vector3 endScale, float duration)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, endScale, t);
                yield return null;
            }
            
            transform.localScale = endScale;
        }
        
        /// <summary>
        /// 设置排序层级
        /// </summary>
        /// <param name="sortingOrder">排序层级</param>
        public void SetSortingOrder(int sortingOrder)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = sortingOrder;
            }
        }
        
        /// <summary>
        /// 设置排序层级名称
        /// </summary>
        /// <param name="sortingLayerName">排序层级名称</param>
        public void SetSortingLayer(string sortingLayerName)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = sortingLayerName;
            }
        }
        
        private void OnDestroy()
        {
            // 清理创建的纹理
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                DestroyImmediate(spriteRenderer.sprite.texture);
            }
        }
    }
}

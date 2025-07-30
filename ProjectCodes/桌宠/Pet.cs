using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class Pet : MonoBehaviour
{
    void OnMouseDown()
    {
        // 生成随机颜色
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        
        // 获取Renderer组件并应用颜色
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = randomColor;
        }
    }
}

using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed = 2f; // 移动速度

    void Start()
    {
        Destroy(gameObject, 10f); // 10秒后销毁
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime); // 向右移动
    }
}

using UnityEngine;
using YanGameFrameWork.ObjectPoolSystem;

public class ObjectPoolExample : MonoBehaviour
{
    public GameObject enemyPrefab; // 敌人预制体

    void Start()
    {
        // 注册对象池,这里也可以不写，会自动注册
        ObjectPoolController.Instance.RegisterPool(enemyPrefab);

        // 获取对象
        GameObject enemy = ObjectPoolController.Instance.GetItem(enemyPrefab);
        enemy.transform.position = new Vector3(0, 0, 0);

        // 使用对象
        // ... 进行一些操作 ...
        // 返回对象到对象池
        ObjectPoolController.Instance.ReturnItem(enemy, enemyPrefab);
    }
}

/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-03
 * Description: 对象池的基类
 *
 ****************************************************************************/
using UnityEngine;
using UnityEngine.Pool;

namespace YanGameFrameWork.ObjectPoolSystem
{
    public class ObjectPoolBase
    {
        private const int _initialPoolSize = 100; // 初始对象池大小
        private const int _maxPoolSize = 10000; // 最大对象池大小

        private GameObject _objectPrefab; // 预制体，用于创建新的Text对象

        private ObjectPool<GameObject> _objPool;
        /// <summary>
        /// 获取对象池中对象的总数量
        /// </summary>
        public int CountAll => _objPool.CountAll;

        /// <summary>
        /// 获取当前使用的对象数量
        /// </summary>
        public int ActiveCount => _objPool.CountActive;

        /// <summary>
        /// 初始化并创建对象池
        /// </summary>
        /// <param name="objectPrefab">这个对象池的预制体</param>
        public ObjectPoolBase(GameObject objectPrefab)
        {
            this._objectPrefab = objectPrefab;
            _objPool = new ObjectPool<GameObject>(CreateNewObject, OnGet, OnRelease, OnDestroyObject, true, _initialPoolSize, _maxPoolSize);
        }

        protected void OnGet(GameObject obj)
        {
            obj.SetActive(true);
        }


        protected void OnRelease(GameObject obj)
        {
            obj.SetActive(false);
        }
        protected void OnDestroyObject(GameObject obj)
        {
            GameObject.Destroy(obj);
        }
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <returns> 返回一个对象 </returns>
        public GameObject GetItem()
        {
            return _objPool.Get();
        }

        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <returns> 返回一个对象 </returns>
        private GameObject CreateNewObject()
        {

            GameObject obj = _objectPrefab as GameObject;
            GameObject newObject = GameObject.Instantiate(obj);
            newObject.name = $"{obj.name}_{_objPool.CountAll}";
            newObject.transform.SetParent(ObjectPoolController.Instance.PoolContainer);
            return newObject;

        }

        /// <summary>
        /// 回收对象到对象池
        /// </summary>
        /// <param name="item"> 要回收的对象 </param>
        public void ReturnItem(GameObject item)
        {
            _objPool.Release(item);
        }
    }
}
/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-07
 * Description: 对象池系统，负责管理对象池。包括创建，释放，获取，统计等。
 *
 ****************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using YanGameFrameWork.Singleton;
namespace YanGameFrameWork.ObjectPoolSystem
{
    public class ObjectPoolController : Singleton<ObjectPoolController>
    {
        private Dictionary<GameObject, ObjectPoolBase> _objectPoolsNew;

        private Transform _poolContainer;
        public Transform PoolContainer
        {
            get
            {
                _poolContainer ??= new GameObject("ObjectPool").transform;
                return _poolContainer;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            YanGF.Debug.Log(nameof(ObjectPoolController), "初始化对象池");
            _objectPoolsNew = new Dictionary<GameObject, ObjectPoolBase>();
        }



        /// <summary>
        /// 注册对象池，并传入对象池的的对象
        /// </summary>
        /// <param name="prefab">预制体</param>
        public void RegisterPool(GameObject prefab)
        {
            if (!_objectPoolsNew.ContainsKey(prefab))
            {
                _objectPoolsNew[prefab] = new ObjectPoolBase(prefab);
            }
        }

        /// <summary>
        /// 获取对象池中的对象，需要传入注册时的预制体
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <returns>对象</returns>
        public GameObject GetItem(GameObject prefab)
        {
            if (_objectPoolsNew.TryGetValue(prefab, out var pool))
            {
                return pool.GetItem();
            }

            // 如果对象池不存在，自动创建
            RegisterPool(prefab);
            return _objectPoolsNew[prefab].GetItem();
        }



        /// <summary>
        /// 返回对象池中的对象，需要传入注册时的预制体
        /// </summary>
        /// <param name="item">对象</param>
        /// <param name="prefab">预制体</param>
        public void ReturnItem(GameObject item, GameObject prefab)
        {
            if (_objectPoolsNew.TryGetValue(prefab, out var pool))
            {
                pool.ReturnItem(item);
            }
            else
            {
                YanGF.Debug.LogError(nameof(ObjectPoolController), $"返回对象池错误：未找到预制体 {prefab.name} 对应的对象池");
            }
        }


        /// <summary>
        /// 获取目标对象池中的对象总数
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <returns>对象总数</returns>
        public int GetTotalCount(GameObject prefab)
        {
            return _objectPoolsNew.TryGetValue(prefab, out var pool) ? pool.CountAll : 0;
        }


        /// <summary>
        /// 获取目标对象池中的对象活跃数量
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <returns>对象活跃数量</returns>
        public int GetActiveCount(GameObject prefab)
        {
            return _objectPoolsNew.TryGetValue(prefab, out var pool) ? pool.ActiveCount : 0;
        }


        /// <summary>
        /// 批量统计方法，可以传入多个预制体，来统计多个对象池中的对象总数
        /// </summary>
        /// <param name="prefabs">预制体列表</param>
        /// <returns>对象总数</returns>
        public int GetTotalCount(IEnumerable<GameObject> prefabs)
        {
            int total = 0;
            foreach (var prefab in prefabs)
            {
                total += GetTotalCount(prefab);
            }
            return total;
        }


        /// <summary>
        /// 批量统计方法，可以传入多个预制体，来统计多个对象池中的对象活跃数量
        /// </summary>
        /// <param name="prefabs">预制体列表</param>
        /// <returns>对象活跃数量</returns>
        public int GetActiveCount(IEnumerable<GameObject> prefabs)
        {
            int total = 0;
            foreach (var prefab in prefabs)
            {
                total += GetActiveCount(prefab);
            }
            return total;
        }

    }

}
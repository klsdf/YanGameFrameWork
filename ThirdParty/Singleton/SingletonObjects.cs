using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FrameWork
{
    public class SingletonObjects : MonoBehaviour
    {
        private static SingletonObjects _instance;

        /// <summary>
        /// 存放Mono类的容器
        /// </summary>
        private GameObject _container;

        private readonly ConcurrentDictionary<string, ObjectContainer> _singletonMap = new();

#if UNITY_EDITOR
        [SerializeField]
        private List<string> _singletons = new();
#endif

        /// <summary>
        /// 单例容器是否被创建
        /// </summary>
        public static bool IsCreate { get { return _instance != null; } }

        private void Awake()
        {
            _instance = this;
        }

        /// <summary>
        /// 获取单例字典的索引
        /// </summary>
        internal static string GetMapKey<T>()
        {
            return typeof(T).FullName;
        }

        /// <summary>
        /// 添加一个单例
        /// </summary>
        public static T AddInstance<T>() where T : class
        {
            Type type = typeof(T);
            T instance;
            //如果是Mono类就创建一个容器，把组件挂在上面
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                if (_instance._container == null)
                {
                    GameObject go = new("SingletonContainer");
                    go.transform.SetParent(_instance.transform);
                    _instance._container = go;
                }
                instance = _instance._container.AddComponent(type) as T;
            }
            else
            {
                instance = Activator.CreateInstance(type) as T;
            }

            if (instance is ISingletonInit singleton)
            {
                ISingletonInit.Register register = default;
                singleton.Init(ref register);
            }

            return AddInstance(instance);
        }

        /// <summary>
        /// 手动创建单例
        /// </summary>
        public static T AddInstance<T>(T instance)
        {
            if (instance != null)
            {
                var container = new ObjectContainer(instance);
                string key = GetMapKey<T>();
#if UNITY_EDITOR
                if (_instance == null)
                {
                    Debug.LogError("游戏需要以StartScene启动,或者在Tools勾上PlayModeUseStartScene");
                }
#endif
                if (_instance._singletonMap.TryAdd(key, container))
                {
#if UNITY_EDITOR
                    _instance._singletons.Add(key);
#endif
                }
                else
                {
                    Debug.LogError($"已存在单例:{key}!");
                    throw new InvalidKeyException();
                }
            }
            else
            {
                Debug.Log("注册的单例为空！");
            }

            return instance;
        }


        public static bool TryGetInstanceContainer<T>(out ObjectContainer container) where T : class
        {
            return _instance._singletonMap.TryGetValue(GetMapKey<T>(), out container);
        }

        /// <summary>
        /// 获取单例
        /// </summary>
        public static bool TryGetInstance<T>(out T instance) where T : class
        {
            if (TryGetInstanceContainer<T>(out ObjectContainer container))
            {
                instance = container.Instance as T;
                return true;
            }

            instance = null;
            return false;
        }

        /// <summary>
        /// 移除单例
        /// </summary>
        private static void RemoveInstance(string key)
        {
            if (_instance._singletonMap.TryRemove(key, out var container))
            {
                container.OnDispose?.Invoke();

                object obj = container.Instance;

                if (obj is IDisposable disposableObj)
                {
                    disposableObj.Dispose();
                }

                if (obj is UnityEngine.Object gameObj)
                {
                    GameObject.Destroy(gameObj);
                }

#if UNITY_EDITOR
                _instance._singletons.Remove(key);
#endif
            }
        }

        /// <summary>
        /// 移除单例
        /// </summary>
        public static void RemoveInstance<T>()
        {
            RemoveInstance(GetMapKey<T>());
        }

        /// <summary>
        /// 移除所有单例
        /// </summary>
        public static void RemoveAllInstance()
        {

            foreach (var pair in _instance._singletonMap)
            {
                RemoveInstance(pair.Key);
            }
        }


        public class ObjectContainer
        {
            internal Action OnDispose;

            public object Instance;

            public ObjectContainer(object obj)
            {
                Instance = obj;
            }
        }
    }
}

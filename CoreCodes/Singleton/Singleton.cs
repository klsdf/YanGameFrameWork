/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-24
 * Description: 单例模式
 *
 ****************************************************************************/
using UnityEngine;

namespace YanGameFrameWork.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            YanGF.Debug.LogError(nameof(Singleton<T>), $"[Singleton] 出现了严重错误 - 不应该有超过1个 {typeof(T)}！重新打开场景可能会修复此问题。");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            //  DontDestroyOnLoad(singleton);

                            // Debug.Log("[Singleton] An instance of " + typeof(T) +
                            //     " is needed in the scene, so '" + singleton +
                            //     "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            // Debug.Log("[Singleton] Using instance already created: " +
                            //     _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }


    }

}
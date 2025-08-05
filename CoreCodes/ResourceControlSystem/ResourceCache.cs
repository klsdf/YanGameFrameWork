using System.Collections.Generic;
using UnityEngine;

namespace YanGameFrameWork
{
    public class ResourceCache
    {
        private Dictionary<string, Object> _cache = new Dictionary<string, UnityEngine.Object>();

        // 泛型方法来缓存资源
        public void CacheResource<T>(string path, T resource) where T : UnityEngine.Object
        {
            if (!_cache.ContainsKey(path))
            {
                _cache[path] = resource;
            }
        }

        // 泛型方法来获取缓存的资源
        public T GetCachedResource<T>(string path) where T : UnityEngine.Object
        {
            if (_cache.TryGetValue(path, out UnityEngine.Object resource))
            {
                return resource as T;
            }
            return null;
        }

        public void UnloadResource(string path)
        {
            if (_cache.TryGetValue(path, out UnityEngine.Object resource))
            {
                Resources.UnloadAsset(resource);
                _cache.Remove(path);
            }
        }
    }
}
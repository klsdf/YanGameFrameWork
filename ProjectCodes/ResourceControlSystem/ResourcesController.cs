/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-31
 * Description: 读取系统，支持资源的缓存
 *
 ****************************************************************************/

using UnityEngine;
using System;
using System.Threading.Tasks;
using YanGameFrameWork.CoreCodes;
namespace YanGameFrameWork.ResourceControlSystem
{
    public class ResourcesController : Singleton<ResourcesController>
    {
        private  readonly ResourceCache _cache = new ResourceCache();

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        public  bool ResourceExists<T>(string path, string name = null) where T : UnityEngine.Object
        {
            string fullPath = name != null ? path + name : path;
            return Resources.Load<T>(fullPath) != null;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="name">资源名称</param>
        /// <returns>资源</returns>
        /// <exception cref="Exception">如果资源不存在，则抛出异常</exception>
        public T LoadResource<T>(string path, string name = null) where T : UnityEngine.Object
        {
            string fullPath = name != null ? path + name : path;
            T resource = _cache.GetCachedResource<T>(fullPath);
            if (resource == null)
            {
                resource = Resources.Load<T>(fullPath);
                if (resource == null)
                {
                    // throw new Exception($"没有找到{fullPath}的{typeof(T).Name}");
                    YanGF.Debug.LogWarning(nameof(ResourcesController), "没有找到" + fullPath + "的" + typeof(T).Name);
                    return null;
                }
                _cache.CacheResource(fullPath, resource);
            }
            return resource;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="name">资源名称</param>
        /// <returns>资源</returns>
        /// <exception cref="Exception">如果资源不存在，则抛出异常</exception>
        public  async Task<T> LoadResourceAsync<T>(string path, string name = null) where T : UnityEngine.Object
        {
            string fullPath = name != null ? path + name : path;
            T resource = _cache.GetCachedResource<T>(fullPath);
            if (resource == null)
            {
                var request = Resources.LoadAsync<T>(fullPath);
                await Task.Yield();
                while (!request.isDone)
                {
                    await Task.Yield();
                }
                resource = request.asset as T;
                if (resource == null)
                {
                    throw new Exception($"没有找到{fullPath}的{typeof(T).Name}");
                }
                _cache.CacheResource(fullPath, resource);
            }
            return resource;
        }


        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="name">资源名称</param>

        public void UnloadResource(string path, string name = null)
        {
            string fullPath = name != null ? path + name : path;
            _cache.UnloadResource(fullPath);
        }
    }
}
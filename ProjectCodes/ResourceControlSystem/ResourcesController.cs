/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-31
 * Description: 读取系统，支持资源的缓存
 *
 * 修改记录：
 * 2025-04-17 闫辰祥 添加TryLoadResource方法，并且删除了加载资产时要传入path和name的复杂逻辑，现在只接受一个path
 ****************************************************************************/

using UnityEngine;
using System;
using System.Threading.Tasks;
using YanGameFrameWork.Singleton;
namespace YanGameFrameWork.ResourceControlSystem
{
    public class ResourcesController : Singleton<ResourcesController>
    {
        private  readonly ResourceCache _cache = new ResourceCache();

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        public  bool ResourceExists<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load<T>(path) != null;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="name">资源名称</param>
        /// <returns>资源</returns>
        /// <exception cref="Exception">如果资源不存在，则抛出异常</exception>
        public T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            T resource = _cache.GetCachedResource<T>(path);
            if (resource == null)
            {
                resource = Resources.Load<T>(path);
                if (resource == null)
                {
                    YanGF.Debug.LogWarning(nameof(ResourcesController), "没有找到" + path + "的" + typeof(T).Name);
                    throw new Exception($"没有找到{path}的{typeof(T).Name}");
                }
                _cache.CacheResource(path, resource);
            }
            return resource;
        }


        /// <summary>
        /// 尝试加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="name">资源名称</param>
        /// <returns>资源，但是没有找到资源时返回null</returns>

        public bool TryLoadResource<T>(string path,out T resource) where T : UnityEngine.Object
        {
            string fullPath = path;
            resource = _cache.GetCachedResource<T>(fullPath);
            if (resource == null)
            {
                resource = Resources.Load<T>(fullPath);
                if (resource == null)
                {
                    return false;
                }
                _cache.CacheResource(fullPath, resource);
            }
            return true;
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
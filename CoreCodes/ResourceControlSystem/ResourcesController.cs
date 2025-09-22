/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-31
 * Description: 读取系统，支持资源的缓存
 *
 * 修改记录：
 * 2025-04-17 闫辰祥 添加TryLoadResource方法，并且删除了加载资产时要传入path和name的复杂逻辑，现在只接受一个path
 * 2025-09-22 闫辰祥 修复LoadFromAddressables方法的类型匹配问题，支持直接加载指定类型的资源，并添加智能回退机制
 
 ****************************************************************************/

using UnityEngine;
using System;
using System.Threading.Tasks;
using YanGameFrameWork.Singleton;
using YanGameFrameWork.UISystem;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;





namespace YanGameFrameWork
{
    public class ResourcesController : Singleton<ResourcesController>
    {
        private readonly ResourceCache _cache = new ResourceCache();

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        public bool ResourceExists<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load<T>(path) != null;
        }


#region 加载Resources中的资源
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

        public bool TryLoadResource<T>(string path, out T resource) where T : UnityEngine.Object
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
        public async Task<T> LoadResourceAsync<T>(string path, string name = null) where T : UnityEngine.Object
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


        #endregion

#region 加载Addressable Assets中的资源



        /// <summary>
        /// 从Addressable Assets中加载指定类型的资源
        /// </summary>
        /// <typeparam name="T">资源类型,直接指定需要的就行</typeparam>
        /// <param name="addressableName">资源路径，也就是Addressable Name</param>
        /// <returns>加载的资源，如果找不到则返回null</returns>
        public T LoadFromAddressables<T>(string addressableName) where T : UnityEngine.Object
        {
            try
            {
                // 检查缓存
                T cachedResource = _cache.GetCachedResource<T>(addressableName);
                if (cachedResource != null)
                {
                    return cachedResource;
                }

                // 从Addressables加载资源
                T result = null;
                
                // 如果请求的是GameObject类型，直接加载GameObject
                if (typeof(T) == typeof(GameObject))
                {
                    var loadOperation = Addressables.LoadAssetAsync<GameObject>(addressableName);
                    loadOperation.WaitForCompletion();
                    
                    if (loadOperation.Status == AsyncOperationStatus.Succeeded && loadOperation.Result != null)
                    {
                        result = loadOperation.Result as T;
                    }
                    Addressables.Release(loadOperation);
                }
                else
                {
                    // 对于其他类型，先尝试直接加载
                    var loadOperation = Addressables.LoadAssetAsync<T>(addressableName);
                    loadOperation.WaitForCompletion();
                    
                    if (loadOperation.Status == AsyncOperationStatus.Succeeded && loadOperation.Result != null)
                    {
                        result = loadOperation.Result;
                    }
                    else
                    {
                        // 如果直接加载失败，尝试从GameObject中获取组件
                        var gameObjectOperation = Addressables.LoadAssetAsync<GameObject>(addressableName);
                        gameObjectOperation.WaitForCompletion();
                        
                        if (gameObjectOperation.Status == AsyncOperationStatus.Succeeded && gameObjectOperation.Result != null)
                        {
                            T component = gameObjectOperation.Result.GetComponent<T>();
                            if (component != null)
                            {
                                result = component;
                            }
                        }
                        Addressables.Release(gameObjectOperation);
                    }
                    Addressables.Release(loadOperation);
                }

                if (result != null)
                {
                    // 缓存资源
                    _cache.CacheResource(addressableName, result);
                    return result;
                }
            }
            catch (System.Exception e)
            {
                YanGF.Debug.LogError(nameof(ResourcesController), $"从Addressable Assets加载资源时出错: {e.Message}");
            }

            return null;
        }



        /// <summary>
        /// 异步从Addressable Assets中加载指定类型的资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源名称</param>
        /// <returns>加载的资源</returns>
        public async Task<T> LoadFromAddressablesAsync<T>(string assetName) where T : UnityEngine.Object
        {
            try
            {
                // 检查缓存
                T cachedResource = _cache.GetCachedResource<T>(assetName);
                if (cachedResource != null)
                {
                    return cachedResource;
                }

                // 从Addressables异步加载资源
                var loadOperation = Addressables.LoadAssetAsync<T>(assetName);

                while (!loadOperation.IsDone)
                {
                    await Task.Yield();
                }

                if (loadOperation.Status == AsyncOperationStatus.Succeeded && loadOperation.Result != null)
                {
                    // 缓存资源
                    _cache.CacheResource(assetName, loadOperation.Result);
                    Addressables.Release(loadOperation);
                    return loadOperation.Result;
                }

                Addressables.Release(loadOperation);
            }
            catch (System.Exception e)
            {
                YanGF.Debug.LogError(nameof(ResourcesController), $"从Addressable Assets异步加载资源时出错: {e.Message}");
            }

            return null;
        }

#endregion


    }
}

/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-28
 * Description: 模块控制器，Module是指游戏的数据。所有的游戏数据都应该继承自YanModelBase，理论上是这样（
 数据本身应该也是单例的，一个数据类型只能注册一次
 *
 ****************************************************************************/

using System.Collections.Generic;
using System;
using UnityEngine;
using YanGameFrameWork.CoreCodes;

namespace YanGameFrameWork.ModelControlSystem
{


    public class ModelController : Singleton<ModelController>
    {


        private Dictionary<Type, YanModelBase> _modules = new Dictionary<Type, YanModelBase>();


        // 在Inspector中显示和编辑数据


        /// <summary>
        /// 单纯注册模块，如果模块已经注册过，则不会注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module">要注册的模块</param>
        /// <returns>注册的模块</returns>
        public T RegisterModule<T>(T module) where T : YanModelBase, new()
        {
            Type moduleType = module.GetType();
            if (HasModule<T>())
            {
                YanGF.Debug.LogWarning(nameof(ModelController), $"模块 {moduleType.Name} 已经注册过了");
                return GetModule<T>();
            }
            _modules[moduleType] = module;
            YanGF.Debug.Log(nameof(ModelController), $"模块 {moduleType.Name} 注册成功");
            return module;
        }


        /// <summary>
        /// 注册或更新模块，如果模块已经注册过，则更新模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module">要注册的模块</param>
        /// <returns>更新或注册的模块</returns>
        public T RegisteOrUpdateModule<T>(T module) where T : YanModelBase
        {
            Type moduleType = module.GetType();
            if (HasModule<T>())
            {
                _modules[moduleType] = module;
                YanGF.Debug.Log(nameof(ModelController), $"模块 {moduleType.Name} 更新成功");
            }
            else
            {
                _modules[moduleType] = module;
                YanGF.Debug.Log(nameof(ModelController), $"模块 {moduleType.Name} 注册成功");
            }
            return module;
        }

        /// <summary>
        /// 获取模块，支持接口和具体类型，如果模块不存在，则创建一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetModule<T>() where T : YanModelBase, new()
        {
            Type requestType = typeof(T);

            // 直接查找完全匹配的类型
            if (_modules.TryGetValue(requestType, out YanModelBase exactModule))
            {
                return exactModule as T;
            }

            // 如果是接口，查找实现该接口的模块
            if (requestType.IsInterface)
            {
                foreach (var module in _modules.Values)
                {
                    if (requestType.IsAssignableFrom(module.GetType()))
                    {
                        return module as T;
                    }
                }
            }
            YanGF.Debug.LogError(nameof(ModelController), $"未找到模块 {requestType.Name}");



            // 如果模块不存在，则创建一个
            T newModule = new T();
            RegisterModule(newModule);
            return newModule;

            // return null;
        }


        /// <summary>
        /// 移除模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool RemoveModule<T>() where T : class
        {
            Type moduleType = typeof(T);
            if (_modules.TryGetValue(moduleType, out YanModelBase module))
            {
                // 如果模块实现了 IDisposable 接口，则调用清理方法
                if (module is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                _modules.Remove(moduleType);
                YanGF.Debug.Log(nameof(ModelController), $"模块 {moduleType.Name} 移除成功");
                return true;
            }
            return false;
        }

        // 检查模块是否存在
        public bool HasModule<T>() where T : class
        {
            return _modules.ContainsKey(typeof(T));
        }



        /// <summary>
        /// 获取所有已注册的模块
        /// </summary>
        /// <returns></returns>
        public Dictionary<Type, YanModelBase> GetAllModules()
        {
            return _modules;
        }

        // 清理所有模块
        public void ClearAllModules()
        {
            foreach (var module in _modules.Values)
            {
                if (module is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _modules.Clear();
            YanGF.Debug.Log(nameof(ModelController), "所有模块已清理");
        }
    }

    // 可选的接口定义
    public interface IInitializable
    {
        void Init();
    }
}
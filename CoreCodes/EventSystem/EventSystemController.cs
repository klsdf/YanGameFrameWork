/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-03
 * Description: 事件系统控制器，使用了发布者-订阅者模式，可以添加无参数事件，带一个参数的事件，带两个参数的事件。
 * 也可以添加一次性事件，事件可以有优先级，优先级高的先执行。
 *
 ****************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using YanGameFrameWork.Singleton;

namespace YanGameFrameWork.EventSystem
{
    /// <summary>
    /// 事件系统控制器：管理事件的注册、触发和移除
    /// </summary>
    public class EventSystemController : Singleton<EventSystemController>
    {
        #region 字段和属性
        // 存储事件及其对应的委托列表
        private readonly Dictionary<string, List<EventHandler>> _eventDictionary = new Dictionary<string, List<EventHandler>>();
        // 存储带参数的事件
        private readonly Dictionary<string, List<Delegate>> _eventParamDictionary = new Dictionary<string, List<Delegate>>();
        // 存储一次性事件
        private readonly HashSet<string> _onceEvents = new HashSet<string>();
        // 是否启用调试日志
        public bool EnableDebugLog { get; set; } = false;
        #endregion

        #region 事件处理器类
        private class EventHandler
        {
            public Action Handler { get; set; }
            public int Priority { get; set; }
            public bool IsOnce { get; set; }
        }
        #endregion

        #region 无参数事件
        /// <summary>
        /// 添加无参数事件监听
        /// </summary>
        public void AddListener(string eventName, Action handler, int priority = 0)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null)
                return;

            if (!_eventDictionary.ContainsKey(eventName))
            {
                _eventDictionary[eventName] = new List<EventHandler>();
            }

            var eventHandler = new EventHandler
            {
                Handler = handler,
                Priority = priority,
                IsOnce = false
            };

            _eventDictionary[eventName].Add(eventHandler);
            // 按优先级排序
            _eventDictionary[eventName].Sort((a, b) => b.Priority.CompareTo(a.Priority));

            if (EnableDebugLog)
                Debug.Log($"Event {eventName} registered with priority {priority}");
        }


        /// <summary>
        /// 添加一次性事件监听
        /// </summary>
        public void AddOnceListener(string eventName, Action handler, int priority = 0)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null)
                return;

            AddListener(eventName, handler, priority);
            _onceEvents.Add(eventName);
        }

        /// <summary>
        /// 触发无参数事件
        /// </summary>
        public void TriggerEvent(string eventName)
        {
            if (!_eventDictionary.ContainsKey(eventName))
                return;

            var handlers = _eventDictionary[eventName];
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                try
                {
                    handlers[i].Handler?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error triggering event {eventName}: {e}");
                }
            }

            // 处理一次性事件
            if (_onceEvents.Contains(eventName))
            {
                RemoveListener(eventName);
                _onceEvents.Remove(eventName);
            }
        }


        /// <summary>
        /// 移除事件监听
        /// </summary>
        public void RemoveListener(string eventName, Action handler = null)
        {
            if (handler == null)
            {
                _eventDictionary.Remove(eventName);
                _eventParamDictionary.Remove(eventName);
            }
            else if (_eventDictionary.ContainsKey(eventName))
            {
                _eventDictionary[eventName].RemoveAll(x => x.Handler == handler);
            }
        }



        #endregion


        #region 一个参数的事件

        /// <summary>
        /// 添加带一个参数的事件监听
        /// </summary>
        public void AddListener<T>(string eventName, Action<T> handler, int priority = 0)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null)
                return;

            if (!_eventParamDictionary.ContainsKey(eventName))
            {
                _eventParamDictionary[eventName] = new List<Delegate>();
            }

            _eventParamDictionary[eventName].Add(handler);
        }



        /// <summary>
        /// 触发带一个参数的事件
        /// </summary>
        public void TriggerEvent<T>(string eventName, T parameter)
        {
            if (!_eventParamDictionary.ContainsKey(eventName))
                return;

            var delegates = _eventParamDictionary[eventName];
            for (int i = delegates.Count - 1; i >= 0; i--)
            {
                if (delegates[i] is Action<T> handler)
                {
                    try
                    {
                        handler(parameter);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error triggering event {eventName} with parameter: {e}");
                    }
                }
            }
        }



        /// <summary>
        /// 移除带一个参数的事件监听
        /// </summary>
        public void RemoveListener<T>(string eventName, Action<T> handler)
        {
            if (_eventParamDictionary.ContainsKey(eventName))
            {
                _eventParamDictionary[eventName].Remove(handler);
            }
        }

        #endregion



        #region 两个参数的事件


        /// <summary>
        /// 添加带两个参数的事件监听
        /// </summary>
        public void AddListener<T1, T2>(string eventName, Action<T1, T2> handler, int priority = 0)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null)
                return;

            if (!_eventParamDictionary.ContainsKey(eventName))
            {
                _eventParamDictionary[eventName] = new List<Delegate>();
            }

            _eventParamDictionary[eventName].Add(handler);
        }


        /// <summary>
        /// 触发带两个参数的事件
        /// </summary>
        public void TriggerEvent<T1, T2>(string eventName, T1 parameter1, T2 parameter2)
        {
            if (!_eventParamDictionary.ContainsKey(eventName))
                return;

            var delegates = _eventParamDictionary[eventName];
            for (int i = delegates.Count - 1; i >= 0; i--)
            {
                if (delegates[i] is Action<T1, T2> handler)
                {
                    try
                    {
                        handler(parameter1, parameter2);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error triggering event {eventName} with parameters: {e}");
                    }
                }
            }
        }



        /// <summary>
        /// 移除带两个参数的事件监听
        /// </summary>
        public void RemoveListener<T1, T2>(string eventName, Action<T1, T2> handler)
        {
            if (_eventParamDictionary.ContainsKey(eventName))
            {
                _eventParamDictionary[eventName].Remove(handler);
            }
        }


        #endregion

        #region 生命周期管理
        /// <summary>
        /// 重写 OnDestroy 方法，确保在对象销毁时清理所有事件数据
        /// </summary>
        protected override void OnDestroy()
        {
            // 调用基类的 OnDestroy 方法
            base.OnDestroy();
            // 清理所有事件数据
            ClearAllEvents();
            
            
        }
        #endregion

        #region 调试和维护方法
        /// <summary>
        /// 清理所有事件
        /// </summary>
        public void ClearAllEvents()
        {
            _eventDictionary.Clear();
            _eventParamDictionary.Clear();
            _onceEvents.Clear();
            YanGF.Debug.Log(nameof(EventSystemController), "All events cleared");
        }

        /// <summary>
        /// 获取事件统计信息
        /// </summary>
        public string GetEventStats()
        {
            return $"Total Events: {_eventDictionary.Count + _eventParamDictionary.Count}, " +
                   $"Simple Events: {_eventDictionary.Count}, " +
                   $"Parameterized Events: {_eventParamDictionary.Count}, " +
                   $"Once Events: {_onceEvents.Count}";
        }

        #endregion
    }
}

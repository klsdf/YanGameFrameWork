/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-03
 * Description: 调试系统，负责打印YanGameFramework的日志。
 * 
 * 修改记录：
 * 2025-05-02 闫辰祥 增加了LogAssert和LogException方法，可以打印断言失败信息和异常信息

 ****************************************************************************/
using YanGameFrameWork.Singleton;
using UnityEngine;
using System;
namespace YanGameFrameWork.DebugSystem
{

    public class DebugController : Singleton<DebugController>
    {

        public bool enable = true;


        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="tag">类名，需要用nameof</param>
        /// <param name="message">消息</param>
        public void Log(string tag, string message)
        {
            if (!enable)
            {
                return;
            }
            Debug.Log($"[<color=green><b> {tag}</b></color>] {message}");
        }

        /// <summary>
        /// 打印警告
        /// </summary>
        /// <param name="tag">类名，需要用nameof</param>
        /// <param name="message">消息</param>
        public void LogWarning(string tag, string message)
        {
            if (!enable)
            {
                return;
            }
            Debug.LogWarning($"[<color=yellow><b>{tag}</b></color>] {message}");
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        /// <param name="tag">类名，需要用nameof</param>
        /// <param name="message">消息</param>
        public void LogError(string tag, string message)
        {
            if (!enable)
            {
                return;
            }
            Debug.LogError($"[<color=red><b>{tag}</b></color>] {message}");
        }

        /// <summary>
        /// 打印断言失败信息
        /// </summary>
        /// <param name="tag">类名，需要用nameof</param>
        /// <param name="message">消息</param>
        public void LogAssert(string tag, Func<bool> condition, string message)
        {
            if (!enable)
            {
                return;
            }
            Debug.Assert(condition(), $"[<color=purple><b>{tag}</b></color>] {message}");
        }

        /// <summary>
        /// 打印异常信息
        /// </summary>
        /// <param name="tag">类名，需要用nameof</param>
        /// <param name="exception">异常对象</param>
        public void LogException(string tag, System.Exception exception)
        {
            if (!enable)
            {
                return;
            }
            Debug.LogError($"[<color=orange><b>{tag}</b></color>] Exception: {exception.Message}\n{exception.StackTrace}");
        }

    }
}

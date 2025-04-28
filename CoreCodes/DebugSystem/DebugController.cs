/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-03
 * Description: 调试系统，负责打印YanGameFramework的日志。
 *
 ****************************************************************************/
using YanGameFrameWork.Singleton;
using UnityEngine;
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
            Debug.Log($"[<color=green><b>✔️ {tag}</b></color>] {message}");
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
            Debug.LogWarning($"[<color=yellow><b>⚠️ {tag}</b></color>] {message}");
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
            Debug.LogError($"[<color=red><b>❌ {tag}</b></color>] {message}");
        }



    }
}

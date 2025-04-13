using UnityEngine;

namespace YanGameFrameWork
{
    /// <summary>
    /// 帧率控制器，用于管理游戏的目标帧率
    /// </summary>
    public class FrameRateController : MonoBehaviour
    {
        public enum FrameRateOptions
        {
            逆天无限帧 = 0,
            丝滑30帧 = 30,
            标准60帧 = 60,
            德芙般的120帧 = 120
        }

        // 使用 SerializeField 特性使枚举在 Inspector 中可见
        [SerializeField]
        [Header("游戏帧率控制")]
        private FrameRateOptions _targetFrameRate = FrameRateOptions.标准60帧;

        void Start()
        {
            // 设置应用程序的目标帧率
            Application.targetFrameRate = (int)_targetFrameRate;
        }

        // 当 Inspector 中的值发生改变时调用
        void OnValidate()
        {
            // 更新应用程序的目标帧率
            Application.targetFrameRate = (int)_targetFrameRate;
        }
    }
}
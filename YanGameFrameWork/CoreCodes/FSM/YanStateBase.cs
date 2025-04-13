/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-24
 * Description: 状态基类
 *
 ****************************************************************************/
namespace YanGameFrameWork.FSM
{
    public abstract class YanStateBase
    {
        /// <summary>
        /// 进入状态时调用
        /// </summary>
        public virtual void OnEnter()
        {

        }

        /// <summary>
        /// 更新状态时调用
        /// </summary>
        public virtual void OnUpdate()
        {

        }
        /// <summary>
        /// 退出状态时调用
        /// </summary>
        public virtual void OnExit()
        {

        }


        /// <summary>
        /// 当状态没有被退出，而是被压到栈时调用
        /// </summary>
        public virtual void OnPause()
        {

        }

        /// <summary>
        /// 当其它的状态都出栈了，重新调用当前状态时
        /// </summary>
        public virtual void OnResume()
        {

        }
    }


}
/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-28
 * Description: Module的基类。所有的游戏数据都应该继承自YanModelBase，理论上是这样（
 *
 * 修改记录：
 * 2025-04-28 闫辰祥 添加克隆方法，主要是为了兼容ModelController的RegisteOrUpdateModule，
因为upadte传进来的时候会传一个新的对象，而直接赋值会导致其OnDataChanged的事件都丢了，所以需要一个克隆方法来兼容。
 ****************************************************************************/

using System;
namespace YanGameFrameWork.ModelControlSystem
{

    /// <summary>
    /// 数据基类
    /// </summary>
    public abstract class YanModelBase
    {
        protected event Action<YanModelBase> OnDataChanged;

        /// <summary>
        /// 通知数据变化
        /// </summary>
        /// <param name="newdata"></param>
        protected void NotifyDataChanged(YanModelBase newdata)
        {
            OnDataChanged?.Invoke(newdata);
        }


        /// <summary>
        /// 添加数据变化监听，需要手动把类型as为子类的类型
        /// </summary>
        /// <param name="listener"></param>
        public void AddDataChangedListener(Action<YanModelBase> listener)
        {
            OnDataChanged += listener;
        }


        /// <summary>
        /// 克隆方法，把参数中的model的值赋值给当前对象
        /// 主要是为了兼容ModelController的RegisteOrUpdateModule，
        /// 因为upadte传进来的时候会传一个新的对象，而直接赋值会导致其OnDataChanged的事件都丢了，所以需要一个克隆方法来兼容。
        /// </summary>
        /// <returns></returns>
        public abstract YanModelBase Clone(YanModelBase model);


    }
}
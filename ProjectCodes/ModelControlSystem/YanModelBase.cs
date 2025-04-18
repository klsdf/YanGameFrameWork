/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-28
 * Description: Module的基类。所有的游戏数据都应该继承自YanModelBase，理论上是这样（
 *
 ****************************************************************************/

using System;
namespace YanGameFrameWork.ModelControlSystem
{
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



    }
}
/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-20
 * Description: UI面板基类，所有的ui面板都应该继承这个类，面板本身不应该具有逻辑或是显示任何元素，
 * 所有的元素都应该由UIElementBase来实现。
 *
 * 2025-05-23 15:36 添加本地化支持，让本地化的语言变化时，自动调用OnLocalize方法
 ****************************************************************************/
using UnityEngine;
namespace YanGameFrameWork.UISystem
{
    /// <summary>
    /// UI面板基类
    /// </summary>
    public abstract class UIPanelBase : MonoBehaviour
    {

        protected virtual void Start()
        {
            OnLocalize();
        }


        void OnEnable()
        {
            YanGF.Localization.OnLanguageChanged += OnLocalize;
        }

        void OnDisable()
        {
            YanGF.Localization.OnLanguageChanged -= OnLocalize;
        }


        public virtual void ChildStart()
        {

        }

        public virtual void OnEnter()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnExit()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnPause()
        {

        }

        public virtual void OnResume()
        {
            Debug.Log("OnResume");
        }


        public abstract void OnLocalize();
    }

}
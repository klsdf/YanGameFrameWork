/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-28
 * Description: UI控制器，主要用一个类似栈的结构来管理UI面板，进入是默认进栈，退出时考虑了特殊情况，
 * 允许不按照栈的顺序退出
 * 需要在inspector中注册需要管理的UI面板，或者在Resources中注册
 *
 * 修改记录：
 * 2025-04-10 闫辰祥 添加了从Resources中加载UI面板的功能，不用每次都手动注册面板，把面板都放场景中了
 ****************************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;
using YanGameFrameWork.CoreCodes;
namespace YanGameFrameWork.UISystem
{
    public class UIController : Singleton<UIController>
    {
        [Header("UI面板的路径")]
        /// <summary>
        /// UI面板的路径
        /// </summary>
        public string UIPanelPath = "UIPanels/";

        [SerializeField]
        private List<UIPanelBase> _registerPanels = new List<UIPanelBase>();

        [SerializeField]
        private List<UIPanelBase> _activePanels = new List<UIPanelBase>();


        [SerializeField]
        private List<UIElementBase> _registerElements = new List<UIElementBase>();

        [SerializeField]
        private List<UIElementBase> _activeElements = new List<UIElementBase>();


        /// <summary>
        /// UI根节点，默认是UI控制器的第一个子物体
        /// </summary>
        private Transform _uiRoot;

        protected override void Awake()
        {
            base.Awake();
            _uiRoot = transform;
        }


        #region UI面板的操作


        private UIPanelBase FindPanelByType(Type panelType)
        {
            foreach (UIPanelBase panel in _registerPanels)
            {
                if (panel.GetType() == panelType)
                {
                    return panel;
                }
            }

            // 如果找不到，从Resources中加载
            string panelName = panelType.Name;
            UIPanelBase[] loadedPanels = Resources.LoadAll<UIPanelBase>($"{UIPanelPath}");
            foreach (UIPanelBase panel in loadedPanels)
            {
                if (panel.GetType() == panelType)
                {
                    UIPanelBase instantiatedPanel = Instantiate(panel.gameObject, _uiRoot).GetComponent<UIPanelBase>();
                    RegisterPanel(instantiatedPanel);
                    return instantiatedPanel;
                }
            }


            YanGF.Debug.LogError(nameof(UIController), "找不到面板：" + panelType);
            return null;
        }

        public void RegisterPanel(UIPanelBase panel)
        {
            _registerPanels.Add(panel);
        }


        public void PopPanel()
        {
            if (_activePanels.Count == 0)
            {
                YanGF.Debug.LogWarning(nameof(UIController), "没有面板可以退出");
                return;
            }

            UIPanelBase temp = PeekPanel();
            temp.OnExit();
            _activePanels.RemoveAt(_activePanels.Count - 1);
        }


        public UIPanelBase PushPanel<T>() where T : UIPanelBase
        {
            UIPanelBase tempPanel = PeekPanel();
            tempPanel?.OnPause();
            UIPanelBase panel = FindPanelByType(typeof(T));
            _activePanels.Add(panel);
            panel.OnEnter();
            return panel;
        }

        public UIPanelBase PeekPanel()
        {
            if (_activePanels.Count == 0)
            {
                return null;
            }
            return _activePanels[_activePanels.Count - 1];
        }


        #endregion


        #region UI元素的操作


        private UIElementBase FindElementByType(Type elementType)
        {
            foreach (UIElementBase element in _registerElements)
            {
                if (element.GetType() == elementType)
                {
                    return element;
                }
            }

            // 如果找不到，从Resources中加载
            string elementName = elementType.Name;
            UIElementBase[] loadedElements = Resources.LoadAll<UIElementBase>($"{UIPanelPath}");
            foreach (UIElementBase element in loadedElements)
            {
                if (element.GetType() == elementType)
                {

                    Transform elementContainer = PeekPanel()?.transform;
                    if (elementContainer == null)
                    {
                        YanGF.Debug.LogError(nameof(UIController), "没有找到元素的容器");
                        return null;
                    }
                    UIElementBase instantiatedElement = Instantiate(element.gameObject, elementContainer).GetComponent<UIElementBase>();
                    RegisterElement(instantiatedElement);
                    return instantiatedElement;
                }
            }


            YanGF.Debug.LogError(nameof(UIController), "找不到元素：" + elementType);
            return null;
        }

        public void RegisterElement(UIElementBase element)
        {
            _registerElements.Add(element);
        }



        public void PopElement()
        {
            if (_activeElements.Count == 0)
            {
                YanGF.Debug.LogWarning(nameof(UIController), "没有元素可以退出");
                return;
            }

            UIElementBase temp = PeekElement();
            temp.OnExit();
            _activeElements.RemoveAt(_activeElements.Count - 1);
        }


        public UIElementBase PushElement<T>() where T : UIElementBase
        {
            UIElementBase tempElement = PeekElement();
            tempElement?.OnPause();
            UIElementBase element = FindElementByType(typeof(T));
            _activeElements.Add(element);
            element.OnEnter();
            return element;
        }

        public UIElementBase PeekElement()
        {
            if (_activeElements.Count == 0)
            {
                return null;
            }
            return _activeElements[_activeElements.Count - 1];
        }



        #endregion


        private void Start()
        {
            ClearScenePanels();
        }


        private void ClearScenePanels()
        {
            foreach (UIPanelBase panel in _registerPanels)
            {
                panel.OnExit();
            }
        }


        public void ClearAllPanels()
        {
            while (_activePanels.Count > 0)
            {
                PopPanel();
            }



        }

    }

}
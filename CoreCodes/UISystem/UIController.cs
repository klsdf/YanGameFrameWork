/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-28
 * Description: UI控制器，主要用一个类似栈的结构来管理UI面板，进入是默认进栈，退出时考虑了特殊情况，
 * 允许不按照栈的顺序退出
 * 需要在inspector中注册需要管理的UI面板，或者在Resources中注册
 *
 * 修改记录：
 * 2025-04-10 闫辰祥 添加了从Resources中加载UI面板的功能，不用每次都手动注册面板，把面板都放场景中了
 * 2025-04-28 闫辰祥 增加了mainCanvas。在每次pushPanel时，检查根节点是否有Canvas组件，如果没有，则将其加入到_mainCanvas下
 * 2025-04-29 闫辰祥 增加了PushPanel的参数，可以指定父级
 ****************************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;
using YanGameFrameWork.Singleton;
namespace YanGameFrameWork.UISystem
{
    public class UIController : Singleton<UIController>
    {
        [Header("UI面板的路径")]
        /// <summary>
        /// UI面板的路径
        /// </summary>
        public string UIPanelPath = "UIPanels/";
        [Header("UI元素的路径")]
        /// <summary>
        /// UI元素的路径
        /// </summary>
        public string UIElementPath = "UIElements/";

        [Header("UI预制体，不在场景中的")]
        /// <summary>
        /// UIpanel的预制体，直接从project拖入
        /// </summary>
        public List<UIPanelBase> UIPanelPrefabs = new List<UIPanelBase>();

        [SerializeField]
        [Header("注册的UI面板，注册的对象需要在场景上")]
        private List<UIPanelBase> _registerPanels = new List<UIPanelBase>();

        [SerializeField]
        private List<UIPanelBase> _activePanels = new List<UIPanelBase>();

        /// <summary>
        /// 弹窗的UI面板
        /// </summary>
        [SerializeField]
        private List<UIPanelBase> _activePopupPanels = new List<UIPanelBase>();


        [SerializeField]
        private List<UIElementBase> _registerElements = new List<UIElementBase>();

        [SerializeField]
        private List<UIElementBase> _activeElements = new List<UIElementBase>();







        /// <summary>
        /// UI根节点，默认是UI控制器的第一个子物体
        /// </summary>
        private Transform _uiRoot;

        private Transform _popCanvas;
        private Transform _mainCanvas;

        protected override void Awake()
        {
            base.Awake();
            _uiRoot = transform;
            _popCanvas = transform.GetChild(0);
            _mainCanvas = transform.GetChild(1);
        }


        #region UI面板的操作


        private UIPanelBase FindPanelByType(Type panelType)
        {
            // 先从已经注册的UI面板中查找
            foreach (UIPanelBase panel in _registerPanels)
            {
                if (panel.GetType() == panelType)
                {
                    return panel;
                }
            }


            // 如果找不到，从UIPanelPrefabs中查找
            foreach (UIPanelBase panel in UIPanelPrefabs)
            {
                if (panel.GetType() == panelType)
                {
                    UIPanelBase instantiatedPanel = Instantiate(panel.gameObject).GetComponent<UIPanelBase>();
                    RegisterPanel(instantiatedPanel);
                    return instantiatedPanel;
                }
            }

            // 如果找不到，从Resources中加载
            string panelName = panelType.Name;
            UIPanelBase[] loadedPanels = Resources.LoadAll<UIPanelBase>($"{UIPanelPath}");
            foreach (UIPanelBase panel in loadedPanels)
            {
                if (panel.GetType() == panelType)
                {
                    UIPanelBase instantiatedPanel = Instantiate(panel.gameObject).GetComponent<UIPanelBase>();
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


        /// <summary>
        /// 退出一个面板
        /// </summary>
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

            if (_activePanels.Count > 0)
            {
                _activePanels[_activePanels.Count - 1].OnResume();
            }
        }

        /// <summary>
        /// 退出一个面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        public void PopPanel<T>() where T : UIPanelBase
        {
            if (_activePanels.Count == 0)
            {
                YanGF.Debug.LogWarning(nameof(UIController), "没有面板可以退出");
                return;
            }

            foreach (UIPanelBase panel in _activePanels)
            {
                if (panel.GetType() == typeof(T))
                {
                    panel.OnExit();

                    _activePanels.Remove(panel);

                    if (_activePanels.Count > 0)
                    {
                        _activePanels[_activePanels.Count - 1].OnResume();
                    }
                    return;
                }
            }
        }



        /// <summary>
        /// 推入一个弹窗面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <param name="parentTransform">父级</param>
        /// <returns>推入的弹窗面板</returns>
        public UIPanelBase PushPopupPanel<T>(Transform parentTransform = null) where T : UIPanelBase
        {
            UIPanelBase tempPanel = PeekPanel();
            tempPanel?.OnPause();
            UIPanelBase panel = FindPanelByType(typeof(T));

            //调整push进来的panel的父级
            if (parentTransform != null)
            {
                panel.transform.SetParent(parentTransform, false);
            }
            else
            {
                SetPanelParent(panel);

            }


            _activePopupPanels.Add(panel);
            panel.OnEnter();
            return panel;
        }


        /// <summary>
        /// 退出弹窗面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        public void PopPopupPanel<T>() where T : UIPanelBase
        {
            if (_activePopupPanels.Count == 0)
            {
                YanGF.Debug.LogWarning(nameof(UIController), "没有面板可以退出");
                return;
            }

            foreach (UIPanelBase panel in _activePopupPanels)
            {
                if (panel.GetType() == typeof(T))
                {
                    panel.OnExit();
                    _activePopupPanels.Remove(panel);
                    return;
                }
            }
        }



        public UIPanelBase PushPanel<T>(Transform parentTransform = null) where T : UIPanelBase
        {
            UIPanelBase tempPanel = PeekPanel();
            tempPanel?.OnPause();
            UIPanelBase panel = FindPanelByType(typeof(T));




            //调整push进来的panel的父级
            if (parentTransform != null)
            {
                panel.transform.SetParent(parentTransform, false);
            }
            else
            {
                SetPanelParent(panel);
            }


            _activePanels.Add(panel);
            panel.OnEnter();
            return panel;
        }


        private void SetPanelParent(UIPanelBase panel)
        {
            // 检查根节点是否有Canvas组件
            if (panel.transform.GetComponent<Canvas>() == null)
            {
                // 如果没有Canvas组件，将其加入到_mainCanvas下
                panel.transform.SetParent(_mainCanvas, false);
            }
            else
            {
                // 如果存在Canvas组件，将其加入到_uiRoot下
                panel.transform.SetParent(_uiRoot, false);
            }
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



        /// <summary>
        /// 根据类型查找已经注册的UI元素,如果找不到，从Resources中加载
        /// </summary>
        /// <param name="elementType">UI元素类型</param>
        /// <returns>找到的UI元素</returns>
        private UIElementBase FindElementByType(Type elementType)
        {

            // 先从已经注册的UI元素中查找
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

                    if (_popCanvas == null)
                    {
                        YanGF.Debug.LogError(nameof(UIController), "没有找到元素的容器");
                        return null;
                    }
                    UIElementBase instantiatedElement = Instantiate(element.gameObject, _popCanvas).GetComponent<UIElementBase>();
                    RegisterElement(instantiatedElement);
                    return instantiatedElement;
                }
            }


            YanGF.Debug.LogError(nameof(UIController), "找不到元素：" + elementType);
            return null;
        }

        public void RegisterElement(UIElementBase element)
        {
            // 检查是否已经存在相同类型的元素
            if (HasElement(element))
            {
                YanGF.Debug.LogWarning(nameof(UIController), "已经存在相同类型的元素：" + element.GetType());
                return;
            }

            // 如果没有相同类型的元素，则注册
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



        /// <summary>
        /// 推入一个UI元素，默认会从已经注册的UI元素中查找，如果找不到，从Resources中加载
        /// </summary>
        /// <typeparam name="T">UI元素类型</typeparam>
        /// <returns>推入的UI元素</returns>
        public UIElementBase PushElement<T>() where T : UIElementBase
        {
            UIElementBase tempElement = PeekElement();
            tempElement?.OnPause();
            UIElementBase element = FindElementByType(typeof(T));
            _activeElements.Add(element);
            element.OnEnter();
            return element;
        }



        /// <summary>
        /// 传入一个UI预制体，并push，并注册这个元素
        /// </summary>
        /// <param name="element">UI预制体</param>
        public UIElementBase PushElement(UIElementBase elementPrefab)
        {

            UIElementBase element;
            if (HasElement(elementPrefab))
            {
                element = FindElementByType(elementPrefab.GetType());
            }
            else
            {
                element = Instantiate(elementPrefab.gameObject, _popCanvas).GetComponent<UIElementBase>();
                RegisterElement(element);
            }
            UIElementBase tempElement = PeekElement();
            tempElement?.OnPause();
            _activeElements.Add(element);
            element.OnEnter();
            return element;
        }

        private bool HasElement(UIElementBase element)
        {
            foreach (UIElementBase registeredElement in _registerElements)
            {
                if (registeredElement.GetType() == element.GetType())
                {
                    return true;
                }
            }
            return false;
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
                if (panel == null)
                {
                    YanGF.Debug.LogWarning(nameof(UIController), $"{panel.name}面板为空");
                    continue;
                }
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


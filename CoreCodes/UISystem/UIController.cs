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
 * 2025-05-26 闫辰祥 大规模重构，移除了Element的概念，所有的UI都是面板，只是父对象不同。增加了自己指定父对象的位置的参数。
 * 2025-05-30 闫辰祥 添加了IsPanelOpen方法，可以检查指定类型的面板是否打开
 ****************************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;
using YanGameFrameWork.Singleton;



namespace YanGameFrameWork.UISystem
{


    public enum PanelType
    {
        Normal,
        Popup,
        Custom
    }
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
        [Header("当前激活的面板")]
        private List<UIPanelBase> _activePanels = new List<UIPanelBase>();





        /// <summary>
        /// UI根节点，默认是UI控制器的第一个子物体
        /// </summary>
        private Transform _uiRoot;

        private Transform _popCanvas;
        private Transform _mainCanvas;

        protected void Awake()
        {
            _uiRoot = transform;
            _popCanvas = transform.GetChild(0);
            _mainCanvas = transform.GetChild(1);
        }



        public void RegisterPanelPrefab(UIPanelBase panel)
        {
            if (!UIPanelPrefabs.Contains(panel))
            {
                UIPanelPrefabs.Add(panel);
            }
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

            // 如果找不到，从Addressable Assets中加载
            UIPanelBase addressablePanel = YanGF.Resources.LoadFromAddressables<UIPanelBase>(panelType.Name);
            if (addressablePanel != null)
            {
                UIPanelBase instantiatedPanel = Instantiate(addressablePanel.gameObject).GetComponent<UIPanelBase>();
                RegisterPanel(instantiatedPanel);
                return instantiatedPanel;
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
                // YanGF.Debug.LogWarning(nameof(UIController), "没有面板可以退出");
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


        public void PopPanel(UIPanelBase panel)
        {
            if (_activePanels.Count == 0)
            {
                YanGF.Debug.LogWarning(nameof(UIController), "没有面板可以退出");
                return;
            }


            if (!HasPanel(panel))
            {

                YanGF.Debug.LogWarning(nameof(UIController), "没有面板可以退出");
                return;
            }

            foreach (UIPanelBase activePanel in _activePanels)
            {
                if (activePanel.GetType() == panel.GetType())
                {
                    activePanel.OnExit();

                    _activePanels.Remove(activePanel);

                    if (_activePanels.Count > 0)
                    {
                        _activePanels[_activePanels.Count - 1].OnResume();
                    }
                    return;
                }
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
                // YanGF.Debug.LogWarning(nameof(UIController), "没有面板可以退出");
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


        public T GetPanel<T>() where T : UIPanelBase
        {
            return (T)FindPanelByType(typeof(T));
        }


        public T PushPanel<T>(UIPanelBase panel, PanelType panelType = PanelType.Normal) where T : UIPanelBase
        {

            if (panel == null)
            {
                Debug.LogError("无法找到或创建面板：" + typeof(T));
                return null;
            }


            UIPanelBase tempPanel = PeekPanel();
            tempPanel?.OnPause();

            UIPanelBase resultPanel = null;

            if (!HasPanel(panel))
            {
                resultPanel = Instantiate(panel.gameObject).GetComponent<UIPanelBase>();
                print("创建了面板：" + resultPanel.name);
                _registerPanels.Add(resultPanel);
            }
            else
            {
                resultPanel = FindPanelByType(panel.GetType());

            }



            SetPanelParent(resultPanel, panelType);



            _activePanels.Add(resultPanel);
            resultPanel.OnEnter();
            // 5. 返回
            return (T)resultPanel;
        }


        public T PushPanel<T>(PanelType panelType = PanelType.Normal) where T : UIPanelBase
        {
            UIPanelBase tempPanel = PeekPanel();
            tempPanel?.OnPause();
            UIPanelBase panel = FindPanelByType(typeof(T));

            // //调整push进来的panel的父级
            // if (parentTransform != null)
            // {
            //     panel.transform.SetParent(parentTransform, false);
            // }
            // else
            // {
            SetPanelParent(panel, panelType);
            // }


            _activePanels.Add(panel);
            panel.OnEnter();
            return (T)panel;
        }


        public T PushPanel<T>(Transform parentTransform) where T : UIPanelBase
        {
            UIPanelBase tempPanel = PeekPanel();
            tempPanel?.OnPause();
            UIPanelBase panel = FindPanelByType(typeof(T));

            // //调整push进来的panel的父级
            // if (parentTransform != null)
            // {
            //     panel.transform.SetParent(parentTransform, false);
            // }
            // else
            // {
            SetPanelParent(panel, PanelType.Custom, parentTransform);
            // }


            _activePanels.Add(panel);
            panel.OnEnter();
            return (T)panel;
        }



        private bool HasPanel(UIPanelBase panel)
        {
            foreach (UIPanelBase registeredElement in _registerPanels)
            {
                if (registeredElement.GetType() == panel.GetType())
                {
                    return true;
                }
            }
            return false;
        }



        public bool IsPanelOpen<T>() where T : UIPanelBase
        {
            return _activePanels.Exists(panel => panel.GetType() == typeof(T));
        }


        private void SetPanelParent(UIPanelBase panel, PanelType panelType, Transform parentTransform = null)
        {
            if (panelType == PanelType.Custom)
            {
                panel.transform.SetParent(parentTransform, false);
                return;
            }


            // 检查根节点是否有Canvas组件
            if (panel.transform.GetComponent<Canvas>() == null)
            {
                // 如果没有Canvas组件，将其加入到_mainCanvas下
                if (panelType == PanelType.Normal)
                {
                    panel.transform.SetParent(_mainCanvas, false);
                }
                else if (panelType == PanelType.Popup)
                {
                    panel.transform.SetParent(_popCanvas, false);
                }

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
                PopPanel(_activePanels[_activePanels.Count - 1]);
            }



        }

    }

}


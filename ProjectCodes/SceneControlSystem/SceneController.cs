/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-13
 *
 * Description: 场景控制器，负责场景的切换和场景对象的管理。
 * 修改记录：
 * 2025-03-28 闫辰祥 重构了代码，使其不再依赖scenetype来索引，而是使用泛型的list来管理，也就是说需要加入新类型的话只需要继承SceneObjBase即可。
 ****************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using YanGameFrameWork.Singleton;


namespace YanGameFrameWork.SceneControlSystem
{
    //用于控制场景和场景切换的类
    public class SceneController : Singleton<SceneController>
    {

        [SerializeField]
        public List<SceneObjBase> sceneObjList = new List<SceneObjBase>();


        public SceneObjBase StartScene;

        private SceneObjBase _activeScene;
        public SceneObjBase ActiveScene
        {
            get
            {
                return _activeScene;
            }
        }



        void Start()
        {
            foreach (SceneObjBase sceneObj in sceneObjList)
            {
                if (sceneObj == null)
                {
                    YanGF.Debug.LogWarning("SceneController", $"<color=yellow><b>⚠️ 警告</b></color>: Scene对象为空");
                    continue;
                }
            }

            MoveToStartScene();
        }


        public void RegisterScene(SceneObjBase sceneObj)
        {
            sceneObjList.Add(sceneObj);
        }


        /// <summary>
        /// 根据类型获取场景对象
        /// </summary>
        /// <typeparam name="T">一个继承了SceneObjBase的类</typeparam>
        /// <returns>一个继承了SceneObjBase的类</returns>
        public SceneObjBase GetSceneObjByType<T>() where T : SceneObjBase
        {
            foreach (SceneObjBase sceneObj in sceneObjList)
            {
                if (sceneObj.SceneType == typeof(T))
                {
                    return sceneObj;
                }
            }

            return null;
        }


        public void MoveToStartScene()
        {
            MoveToScene(StartScene);
        }


        /// <summary>
        /// 移动到指定场景
        /// </summary>
        /// <typeparam name="T">一个继承了SceneObjBase的类</typeparam>
        public void MoveToScene<T>() where T : SceneObjBase
        {
            foreach (SceneObjBase sceneObj in sceneObjList)
            {
                if(sceneObj == null)
                {
                    YanGF.Debug.LogWarning("SceneController", $"<color=yellow><b>⚠️ 警告</b></color>: Scene对象为空");
                    continue;
                }
                if (sceneObj.SceneType == typeof(T))
                {
                    _activeScene?.OnExit();
                    _activeScene = sceneObj;
                    _activeScene?.OnEnter();
                }
                else
                {
                    sceneObj.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 移动到指定场景
        /// </summary>
        /// <param name="sceneObj">一个继承了SceneObjBase的类</param>
        public void MoveToScene(SceneObjBase sceneObj)
        {
            // 退出当前活动场景
            _activeScene?.OnExit();

            // 设置新的活动场景
            _activeScene = sceneObj;
            _activeScene?.OnEnter();


        }


    }
}
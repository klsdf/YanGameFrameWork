/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-13
 *
 * Description: 场景控制器，负责场景的切换和场景对象的管理。
 * 修改记录：
 * 2025-03-28 闫辰祥 重构了代码，使其不在依赖scenetype来索引，而是使用泛型的list来管理，也就是说需要加入新类型的话只需要继承SceneObjBase即可。
 ****************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using YanGameFrameWork.CoreCodes;


namespace YanGameFrameWork.SceneControlSystem
{
    //用于控制场景和场景切换的类
    public class SceneController : Singleton<SceneController>
    {

        [SerializeField]
        public List<SceneObjBase> sceneObjList = new List<SceneObjBase>();

        private SceneObjBase _activeScene;
        public SceneObjBase ActiveScene
        {
            get
            {
                return _activeScene;
            }
        }

        // public SceneObjBase startScene;


        void Start()
        {
            MoveToStartScene();
        }


        public void RegisterScene(SceneObjBase sceneObj)
        {
            sceneObjList.Add(sceneObj);
        }

        public SceneObjBase GetSceneObjByType<T>()
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
            // MoveToScene<SceneOuterMenu>();
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
                    Debug.LogWarning($"<color=yellow><b>⚠️ 警告</b></color>: Scene对象为空");
                    continue;
                }
                if (sceneObj.SceneType == typeof(T))
                {
                    ActiveScene?.OnExit();
                    _activeScene = sceneObj;
                    ActiveScene?.OnEnter();
                }
            }
        }


    }
}
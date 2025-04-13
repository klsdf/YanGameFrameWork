/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-13
 * Description: 场景对象基类，负责场景的切换和场景对象的管理。
 *
 ****************************************************************************/

using UnityEngine;
namespace YanGameFrameWork.SceneControlSystem
{
    public abstract class SceneObjBase : MonoBehaviour
    {
        public abstract System.Type SceneType { get; } // 获取场景类型

        // void Start()
        // {
        //     SceneController.Instance.RegisterScene(this);
        // }
        // public GameObject sceneObj;
        public virtual void OnEnter()
        {
            gameObject.SetActive(true);
        }
        public virtual void OnExit()
        {
            gameObject.SetActive(false);
        }

    }


}
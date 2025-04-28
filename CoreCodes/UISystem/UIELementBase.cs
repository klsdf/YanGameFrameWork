/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-20
 * Description: UI元素基类，所有的ui元素都应该继承这个类，元素本身不应该具有逻辑或是显示任何元素，
 * 所有的元素都应该由UIElementBase来实现。
 ****************************************************************************/
using UnityEngine;

namespace YanGameFrameWork.UISystem
{
    public class UIElementBase : MonoBehaviour
    {

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

        }

    }

}
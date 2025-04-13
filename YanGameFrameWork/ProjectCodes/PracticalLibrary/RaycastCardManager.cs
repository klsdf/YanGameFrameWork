/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-24
 * Description: 自定义的射线检测管理器，可以识别物体的进入和离开
 * 需要手动调用UpdateCardStates()方法来更新状态，然后这个类会自动识别进入的T和离开的T
 * 并调用对应的onEnter和onLeave方法，将进入或离开的对象传出去
 *
 ****************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System;

namespace YanGameFrameWork.PracticalLibrary
{
    public class RaycastCardManager<T> where T : Component
    {
        private HashSet<T> _cardsInArea = new HashSet<T>();
        private Action<T> _onEnter;
        private Action<T> _onLeave;


        public void SetOnEnterEvent(Action<T> onEnterAction)
        {
            _onEnter = onEnterAction;
        }

        public void SetOnLeaveEvent(Action<T> onLeaveAction)
        {
            _onLeave = onLeaveAction;
        }

        public void UpdateCardStates()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
            HashSet<T> currentHits = new HashSet<T>();

            foreach (var hit in hits)
            {
                T card = hit.collider.GetComponent<T>();
                if (card != null)
                {
                    currentHits.Add(card);
                    if (!_cardsInArea.Contains(card))
                    {
                        _onEnter(card);
                        _cardsInArea.Add(card);
                    }
                }
            }

            _cardsInArea.RemoveWhere(card =>
            {
                if (!currentHits.Contains(card) && card != null)
                {
                    _onLeave(card);
                    return true;
                }
                return false;
            });
        }
    }

}
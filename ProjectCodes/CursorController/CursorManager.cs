using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YanGameFrameWork.FSM;
using YanGameFrameWork.Singleton;

namespace YanGameFrameWork.CursorController
{
    /// <summary>
    /// 切换鼠标样式用的
    /// </summary>
    public class CursorManager : Singleton<CursorManager>
    {
        public CursorData[] cursors;
        private Dictionary<string, CursorData> _cursorDictionary;

        protected YanStateBase _currentState;

        public string CurrentCursorName
        {
            get
            {
                if (_currentState is CursorStateNormal)
                {
                    return Normal;
                }
                else if (_currentState is CursorStateDelete)
                {
                    return Delete;
                }
                else
                {
                    return Normal;
                }
            }
        }


        public const string Normal = "Normal";
        public const string Clicking = "Clicking";
        public const string Delete = "Delete";



        protected override void Awake()
        {
            base.Awake();


            if (cursors == null || cursors.Length == 0)
            {
                return;
            }
            // 初始化字典
            _cursorDictionary = new Dictionary<string, CursorData>();
            foreach (CursorData cursor in cursors)
            {
                _cursorDictionary.Add(cursor.name, cursor);
            }

            // 初始化状态机
            ChangeCursorState(Normal);
        }

        public void ChangeCursorState(string cursorName)
        {
            switch (cursorName)
            {
                case Normal:
                    ChangeState(new CursorStateNormal(this));
                    break;
                case Delete:
                    ChangeState(new CursorStateDelete(this));
                    break;
                default:
                    ChangeState(new CursorStateNormal(this));
                    break;
            }
        }


        /// <summary>
        /// 设置鼠标样式的公共方法
        /// </summary>
        /// <param name="cursorName"></param>
        public void SetCursor(string cursorName)
        {
            if (_cursorDictionary.TryGetValue(cursorName, out CursorData cursor))
            {
                Cursor.SetCursor(cursor.cursorTexture, cursor.hotSpot, CursorMode.Auto);
            }
            else
            {
                YanGF.Debug.LogError(nameof(CursorManager), "cursorName is not found");
            }
        }












        private void Update()
        {
            _currentState?.OnUpdate();
        }
        public void ChangeState(YanStateBase newState)
        {

            if (newState == null)
            {
                YanGF.Debug.LogError(nameof(YanFSMController), "newState is null");
                return;
            }
            _currentState?.OnExit();
            // _states.Remove(_currentState);

            _currentState = newState;
            _currentState.OnEnter();
        }



#if UNITY_EDITOR
        void OnGUI()
        {
            Vector2 mousePos = Event.current.mousePosition;
            float size = 10f;
            Color oldColor = GUI.color;
            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(mousePos.x - 1, mousePos.y - size, 2, size * 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(mousePos.x - size, mousePos.y - 1, size * 2, 2), Texture2D.whiteTexture);
            GUI.color = oldColor;
        }
#endif
    }
}
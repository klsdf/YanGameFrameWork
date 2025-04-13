using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YanGameFrameWork.FSM;


namespace YanGameFrameWork.CursourController
{
    /// <summary>
    /// 切换鼠标样式用的
    /// </summary>
    public class CursorManager : YanFSMController
    {
        public CursorData[] cursors;
        private Dictionary<string, CursorData> _cursorDictionary;

        void Awake()
        {
            // 初始化字典
            _cursorDictionary = new Dictionary<string, CursorData>();
            foreach (CursorData cursor in cursors)
            {
                _cursorDictionary.Add(cursor.name, cursor);
            }

            // 初始化状态机
            ChangeState(new CursorStateNormal(this));
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
        }
    }
}
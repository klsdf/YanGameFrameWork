using UnityEngine;
namespace YanGameFrameWork.CursorController
{
    public class CursorStateDelete : CursorState
    {
        public CursorStateDelete(CursorManager manager) : base(manager) {}

        public override void OnEnter()
        {
            cursorManager.SetCursor(CursorManager.Delete);
            Debug.Log("CursorStateDelete OnEnter");
        }
    }
}
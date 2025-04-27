using UnityEngine;
namespace YanGameFrameWork.CursorController
{
    public class CursorStateNormal : CursorState
    {
        public CursorStateNormal(CursorManager manager) : base(manager) { }

        public override void OnEnter()
        {
            cursorManager.SetCursor(CursorManager.Normal);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.GetMouseButtonDown(0))
            {
                cursorManager.SetCursor(CursorManager.Clicking);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                cursorManager.SetCursor(CursorManager.Normal);
            }


        }
    }
}
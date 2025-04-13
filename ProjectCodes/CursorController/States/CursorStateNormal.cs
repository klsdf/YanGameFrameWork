using UnityEngine;
namespace YanGameFrameWork.CursourController
{
    public class CursorStateNormal : CursorState
    {
        public CursorStateNormal(CursorManager manager) : base(manager) { }

        public override void OnEnter()
        {
            cursorManager.SetCursor("Normal");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.GetMouseButtonDown(0))
            {
                cursorManager.SetCursor("Clicking");
            }
            else if (Input.GetMouseButtonUp(0))
            {
                cursorManager.SetCursor("Normal");
            }


        }
    }
}
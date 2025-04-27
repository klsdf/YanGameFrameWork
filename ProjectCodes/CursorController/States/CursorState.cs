using YanGameFrameWork.FSM;

namespace YanGameFrameWork.CursorController
{
    public abstract class CursorState : YanStateBase
    {
        protected CursorManager cursorManager;

        public CursorState(CursorManager manager)
        {
            cursorManager = manager;
        }

        public override void OnEnter()
        {

        }
        public override void OnUpdate()
        {

        }
        public override void OnExit()
        {
        }
    }
}
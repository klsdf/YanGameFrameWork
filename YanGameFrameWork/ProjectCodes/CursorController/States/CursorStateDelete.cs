namespace YanGameFrameWork.CursourController
{
    public class CursorStateDelete : CursorState
    {
        public CursorStateDelete(CursorManager manager) : base(manager) {}

        public override void OnEnter()
        {
            cursorManager.SetCursor("Delete");
        }
    }
}
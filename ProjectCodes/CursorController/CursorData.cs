using UnityEngine;
namespace YanGameFrameWork.CursorController
{
    [System.Serializable]
    public struct CursorData
    {
        public string name;
        public Texture2D cursorTexture;
        public Vector2 hotSpot;
    }
}
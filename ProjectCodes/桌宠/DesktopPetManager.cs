using UnityEngine;
using System;
using System.Runtime.InteropServices;
public class DesktopPetManager : Singleton<DesktopPetManager>
{

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("Dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, int type);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    private const int GWL_EXSTYLE = -20;
    private const uint WS_EX_LAYERED = 0x80000;
    private const uint WS_EX_TRANSPARENT = 0x20;


    private const int HWND_TOPMOST = -1;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

    private const uint LWA_ALPHA = 0x2;
    private const uint LWA_COLORKEY = 0x1;

    public static IntPtr HWND { get; private set; }
    private bool isTransparent = false;



    private void Start()
    {


#if !UNITY_EDITOR

        HWND = GetActiveWindow();
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(HWND, ref margins);
        SetWindowLong(HWND, GWL_EXSTYLE, WS_EX_LAYERED);

        // 设置透明色为黑色(0)，确保黑色部分透明
        SetLayeredWindowAttributes(HWND, 0, 0, LWA_COLORKEY);

        // SetLayeredWindowAttributes(HWND, 0, 128, LWA_ALPHA);

        // 窗口置顶
        SetWindowPos(HWND, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, 0);
        Application.runInBackground = true;
        Camera.main.backgroundColor = Color.black;
#endif

    }

}
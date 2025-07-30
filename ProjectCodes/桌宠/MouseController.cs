using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;




public class MouseController : Singleton<MouseController>
{
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);



    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const uint KEYEVENTF_KEYDOWN = 0x0000;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    public Vector2 minLimit = new Vector2(100, 100);  // 限制范围的最小值
    public Vector2 maxLimit = new Vector2(500, 500);  // 限制范围的最大值
    // public Image rangeImage;  // 用于显示范围的 UI 元素

    private bool isControlActive = false;  // 控制是否激活

    void Start()
    {
        // StartCoroutine(OpenCommandPromptAndExecuteNode());
        // StartCoroutine(ExecuteSequentially( 2.0f));
    }

    // private IEnumerator ExecuteSequentially(float duration)
    // {
    //     // 测试鼠标移动到多个位置
    //     Vector2[] testPositions = new Vector2[]
    //     {
    //         new Vector2(700, 1065),
    //         // new Vector2(100, 0),
    //     };
    //     // 先执行鼠标移动
    //     yield return StartCoroutine(SmoothMoveMouse(testPositions, duration));

    //     // 然后执行按键序列
    //     byte[] keySequence = new byte[] { 0x5B, 0x0D }; // A, B, C
    //     yield return StartCoroutine(SimulateKeyPressCoroutine(keySequence, 0.5f)); // 每个按键之间间隔0.5秒
    // }

    void Update()
    {
        // 检查是否按下 P 键以切换控制状态
        if (Input.GetKeyDown(KeyCode.P))
        {
            isControlActive = !isControlActive;
        }

        if (isControlActive)
        {
            // 处理范围调整
            // HandleRangeAdjustment();

            Vector3 mousePosition = Input.mousePosition;

            // 限制鼠标在特定范围内
            mousePosition.x = Mathf.Clamp(mousePosition.x, minLimit.x, maxLimit.x);
            mousePosition.y = Mathf.Clamp(mousePosition.y, minLimit.y, maxLimit.y);

            // 设置鼠标位置
            SetCursorPos((int)mousePosition.x, (int)mousePosition.y);

            // // 模拟鼠标点击
            // if (Input.GetMouseButtonDown(0))
            // {
            //     SimulateMouseClick(mousePosition);
            // }

            // // 示例：按下空格键
            // if (Input.GetKeyDown(MyKeyCode.Space))
            // {
            //     SimulateKeyPress(0x20); // 0x20 是空格键的虚拟键码
            // }

            // 更新范围显示
            // UpdateRangeImage();
        }
    }

    // private void UpdateRangeImage()
    // {
    //     if (rangeImage != null)
    //     {pppppppttptpppp
    //         Vector2 size = maxLimit - minLimit;
    //         rangeImage.rectTransform.sizeDelta = size;
    //         rangeImage.rectTransform.position = new Vector3(minLimit.x + size.x / 2, minLimit.y + size.y / 2, 0);
    //     }
    // }

    // 新增的协程，用于丝滑移动鼠标到多个指定位置
    public void MoveMouseToPositions(Vector2[] positions, float duration)
    {
        StartCoroutine(SmoothMoveMouse(positions, duration));
    }

    private IEnumerator SmoothMoveMouse(Vector2[] positions, float duration)
    {
        foreach (var targetPosition in positions)
        {
            Vector2 startPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, t);
                SetCursorPos((int)newPosition.x, (int)newPosition.y);
                yield return null;
            }
        }
        SimulateMouseClick(positions[positions.Length - 1]);

    }

    private void SimulateMouseClick(Vector3 position)
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)position.x, (uint)position.y, 0, UIntPtr.Zero);
        mouse_event(MOUSEEVENTF_LEFTUP, (uint)position.x, (uint)position.y, 0, UIntPtr.Zero);
    }

    public void SimulateKeyPress(MyKeyCode MyKeyCode)
    {
        byte VirtualMyKeyCode = GetVirtualMyKeyCode(MyKeyCode.keyName);
        if (MyKeyCode.isShift == true)
        {
            keybd_event(GetVirtualMyKeyCode("SHIFT"), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        }
        keybd_event(VirtualMyKeyCode, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        keybd_event(VirtualMyKeyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        if (MyKeyCode.isShift == true)
        {
            keybd_event(GetVirtualMyKeyCode("SHIFT"), 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

    }

    // public void SimulateKeyPressSequence(byte[] MyKeyCodes, float interval)
    // {
    //     StartCoroutine(SimulateKeyPressCoroutine(MyKeyCodes, interval));
    // }

    /**
        模拟按键序列
    */
    private IEnumerator SimulateKeyPressCoroutine(MyKeyCode[] MyKeyCodes, float interval)
    {
        foreach (MyKeyCode MyKeyCode in MyKeyCodes)
        {
            SimulateKeyPress(MyKeyCode);
            yield return new WaitForSeconds(interval);
        }
    }


   public class MyKeyCode
    {
        public string keyName;
        public bool isShift;
        public MyKeyCode(string keyName, bool isShift = false)
        {
            this.keyName = keyName;
            this.isShift = isShift;
        }
    }


    public void OpenConsoleAndRun()
    {
        print("OpenConsoleAndRun");
        StartCoroutine(OpenCommandPromptAndExecuteNode());
    }

    /**
        打开命令提示符并执行node
    */
    private IEnumerator OpenCommandPromptAndExecuteNode()
    {
        const byte VK_LWIN = 0x5B; // Windows 键
        const byte VK_R = 0x52;    // R 键

        // 模拟按下 Win + R
        keybd_event(VK_LWIN, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        keybd_event(VK_R, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        keybd_event(VK_R, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

        // 等待一会儿以确保运行窗口打开
        // System.Threading.Thread.Sleep(500);
        yield return new WaitForSeconds(1.0f);

        MyKeyCode[] command_cmd = new MyKeyCode[]
        {
            // new MyKeyCode("WIN", true),
            new MyKeyCode("c"),
            new MyKeyCode("m"),
            new MyKeyCode("d"),
            new MyKeyCode("return"),
        };

        // 输入 "cmd"
        yield return StartCoroutine(SimulateKeyPressCoroutine(command_cmd ,0.1f));


        // 等待命令提示符打开
        // System.Threading.Thread.Sleep(500);
        yield return new WaitForSeconds(1.0f);

        // 获取命令提示符窗口句柄并置于前台
        Process[] processes = Process.GetProcessesByName("cmd");
        if (processes.Length > 0)
        {
            SetForegroundWindow(processes[0].MainWindowHandle);
        }
        MyKeyCode[] commandSequence = new MyKeyCode[]
        {
            new MyKeyCode("return"),
            new MyKeyCode("return"),
            new MyKeyCode("return"),
            new MyKeyCode("return"),
            new MyKeyCode("return"),
            new MyKeyCode("I"),
            new MyKeyCode("P"),
            new MyKeyCode("C"),
            new MyKeyCode("O"),
            new MyKeyCode("N"),
            new MyKeyCode("F"),
            new MyKeyCode("I"),
            new MyKeyCode("G"),
            new MyKeyCode("return"),

            new MyKeyCode("S"), // 'S'
            new MyKeyCode("T"), // 'T'
            new MyKeyCode("A"), // 'A'
            new MyKeyCode("R"), // 'R'
            new MyKeyCode("T"), // 'T'
            new MyKeyCode(" "), // 空格
            new MyKeyCode("\"",true), // '"'
            new MyKeyCode("\"",true), // '"'
            new MyKeyCode(" "), // 空格
            new MyKeyCode("\"",true), // '"'
            new MyKeyCode("f"), // 'f'
            new MyKeyCode("i"), // 'i'
            new MyKeyCode("l"), // 'l'
            new MyKeyCode("e"), // 'e'
            new MyKeyCode(":",true), // ':'
            new MyKeyCode("/"), // '/'
            new MyKeyCode("/"), // '/'
            new MyKeyCode("D"), // 'D'
            new MyKeyCode(":",true), // ':'
            new MyKeyCode("/"), // '/'
            new MyKeyCode("t"), // 't'
            new MyKeyCode("e"), // 'e'
            new MyKeyCode("s"), // 's'
            new MyKeyCode("t"), // 't'
            new MyKeyCode("/"), // '/'
            new MyKeyCode("i"), // 'i'
            new MyKeyCode("n"), // 'n'
            new MyKeyCode("d"), // 'd'
            new MyKeyCode("e"), // 'e'
            new MyKeyCode("x"), // 'x'
            new MyKeyCode("."), // '.'
            new MyKeyCode("h"), // 'h'
            new MyKeyCode("t"), // 't'
            new MyKeyCode("m"), // 'm'
            new MyKeyCode("l"), // 'l'
            new MyKeyCode("\"",true), // '"'
            new MyKeyCode("return"), 
            
            
        };

        // 使用这个序列来模拟按键
        yield return StartCoroutine(SimulateKeyPressCoroutine(commandSequence, 0.1f));
    }

    public byte GetVirtualMyKeyCode(string character)
    {
        character = character.ToUpper();
        switch (character)
        {
            case "SHIFT": return 0x10;
            case "WIN": return 0x5B;
            case "CAPS LOCK ": return 0x14;
            case "RETURN": return 0x0D;
            case "A": return 0x41;
            case "B": return 0x42;
            case "C": return 0x43;
            case "D": return 0x44;
            case "E": return 0x45;
            case "F": return 0x46;
            case "G": return 0x47;
            case "H": return 0x48;
            case "I": return 0x49;
            case "J": return 0x4A;
            case "K": return 0x4B;
            case "L": return 0x4C;
            case "M": return 0x4D;
            case "N": return 0x4E;
            case "O": return 0x4F;
            case "P": return 0x50;
            case "Q": return 0x51;
            case "R": return 0x52;
            case "S": return 0x53;
            case "T": return 0x54;
            case "U": return 0x55;
            case "V": return 0x56;
            case "W": return 0x57;
            case "X": return 0x58;
            case "Y": return 0x59;
            case "Z": return 0x5A;
            case " ": return 0x20;
            case "-": return 0x2D;
            case "\"": return 0xDE;
            case ".": return 0xBE;
            case "(": return 0x28;
            case ")": return 0x29;
            case "+": return 0x2B;
            case "1": return 0x31;
            case "2": return 0x32;
            case "3": return 0x33;
            case "4": return 0x34;
            case "5": return 0x35;
            case "6": return 0x36;
            case "7": return 0x37;
            case "8": return 0x38;
            case "9": return 0x39;
            case "0": return 0x30;
            case ":": return 0xBA;
            case "/": return 0xBF;
            // 添加更多字符映射
            default: return 0x00; // 未知字符
        }
    }
}

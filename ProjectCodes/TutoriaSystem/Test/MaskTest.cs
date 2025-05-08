
using UnityEngine;
using UnityEngine.UI;

public class MaskTest : MonoBehaviour
{

    void Start()
    {
#if USE_LIBTESSDOTNET
        // 这部分代码只会在aaa库存在时编译和执行
        Debug.Log("aaa library is available");

#else
        Debug.Log("aaa library is not available");
        // CallAAAFunction();
#endif
    }
}

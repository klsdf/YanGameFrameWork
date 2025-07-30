using UnityEngine;
using YanGameFrameWork.Editor;

public class TweenExample : MonoBehaviour
{

    public CanvasGroup canvasGroup;



    [Button("AlphaTweenTest")]
    public void AlphaTweenTest()
    {
        YanGF.Tween.Tween(
            canvasGroup,
            item => item.alpha,
            0f,
            1f,
            () => Debug.Log("Tween完成")
        );
    }
    
    [Button("AlphaTweenTest2")]
    public void AlphaTweenTest2()
    {
        YanGF.Tween.Tween(
            canvasGroup,
            item => item.alpha,
            1f,
            1f,
            () => Debug.Log("Tween完成")
        );
    }
}

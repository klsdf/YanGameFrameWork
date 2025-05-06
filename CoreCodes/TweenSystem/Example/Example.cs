using UnityEngine;

public class Example : MonoBehaviour
{

    public CanvasGroup canvasGroup;
    public void Start()
    {
       YanGF.Tween.Tween(
            canvasGroup, 
            item => item.alpha, 
            0f, 
            1f, 
            () => Debug.Log("Tween完成")
        );
    }
}

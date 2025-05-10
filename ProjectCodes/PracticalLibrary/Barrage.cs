/****************************************************************************
 * Author: 闫辰祥
 * Date: 2024-12-24
 * Description: 弹幕类
 *
 * 修改记录:
 * 2025-05-10 闫辰祥 取消了弹幕自己的update，现在由BarrageController统一管理，可以提高性能
 ****************************************************************************/
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 这个类用于控制单个弹幕的行为。
/// </summary>
public class Barrage : MonoBehaviour
{

    private TMP_Text _textComponent; // 文本组件


    private void Awake()
    {
        // 获取TextMeshPro组件
        _textComponent = GetComponentInChildren<TMP_Text>();
    }

    public float speed;
    /// <summary>
    /// 设置弹幕的文本内容。
    /// </summary>
    /// <param name="text">要显示的文本</param>
    public void Init(string text, int fontSize, float speed)
    {
        if (_textComponent != null)
        {
            _textComponent.text = text;
            _textComponent.fontSize = fontSize;
        }
        this.speed = speed;
    }

    // /// <summary>
    // /// 每帧更新弹幕的位置。
    // /// </summary>
    // private void Update()
    // {
    //     // 向左移动弹幕
    //     transform.Translate(Vector3.left * speed * Time.deltaTime);

    //     // 如果弹幕移出屏幕左侧，则销毁
    //     if (transform.position.x < -Screen.width)
    //     {
    //         Destroy(gameObject);
    //     }
    // }
}
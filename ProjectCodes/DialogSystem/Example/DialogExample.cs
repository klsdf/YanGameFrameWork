using System.Collections.Generic;
using UnityEngine;
using YanGameFrameWork.DialogSystem;
using TMPro;
using System;

/// <summary>
/// 对话系统示例脚本。
/// 设计原因：
/// 1) 以最小可行示例演示注册对话块（DialogBlock）与按键名运行顺序对话；
/// 2) 展示如何使用打字机效果输出文本，并在结束时接收回调；
/// 3) 方便美术或策划参考结构，在实际项目中可将数据转为 ScriptableObject 或从配置读入。
/// 
/// 使用步骤：
/// 1. 在场景中创建空物体并挂载本脚本，将 UI 文本组件（TMP_Text）拖到 dialogText；
/// 2. 在 dialogBlocks 中定义若干 DialogBlock，每个包含一段对话（Dialog 列表），并指定一个唯一键名；
/// 3. 在 Start 中调用 DialogController.RegisterDialogBlock 逐个注册；
/// 4. 使用 DialogController.RunSequenceDialog 通过键名运行整段对话，
///    在回调中用 DialogController.StartTypingEffect 实现打字机效果；
/// 5. 结束回调中可执行后续逻辑（如切下一段对话、打开 UI、驱动剧情）。
/// </summary>
public class DialogExample : MonoBehaviour
{
    /// <summary>
    /// 打字显示用的文本组件（TMP_Text）。
    /// </summary>
    public TMP_Text dialogText;

    public void Start()
    {
        // 1) 注册所有对话块（必须先注册，才能通过键名运行）
        foreach (var dialogBlock in dialogBlocks)
        {
            DialogController.Instance.RegisterDialogBlock(dialogBlock);
        }
        
        // 2) 运行一段对话序列：按键名逐条吐出 Dialog
        DialogController.Instance.RunSequenceDialog("DialogExample", (dialog) =>
        {
            // 输出方式 A：直接赋值文本（简单）
            // dialogText.text = dialog.dialog;

            // 输出方式 B：打字机效果（沉浸感更强）
            DialogController.Instance.StartTypingEffect(dialog.dialog, 0.1f, dialogText);
        }, () =>
        {
            Debug.Log("DialogExample end");
        });
    }

    /// <summary>
    /// 示例用的对话角色，项目中可从配置表或角色系统获取。
    /// </summary>
    static DialogCharacter _loli = new DialogCharacter("小萝莉");

    /// <summary>
    /// 本示例中定义的对话块集合。标记为 NonSerialized 以避免被 Unity 序列化；
    /// 实际项目中建议改为 ScriptableObject、表格或外部配置驱动。
    /// </summary>
    [NonSerialized]
    public List<DialogBlock> dialogBlocks = new List<DialogBlock>()
    {
        // 键名："DialogExample" —— 将在 Start 中被运行
        new DialogBlock("DialogExample", new List<Dialog>()
        {
            new Dialog("你好啊", _loli),
            new Dialog("我叫小萝莉", _loli),
            new Dialog("我最喜欢穿白丝", _loli),
            new Dialog("因为我是美味的小萝莉", _loli),
        }),

        // 另一个示例块："DialogExample2" —— 未在示例中自动运行，可按需切换执行
        new DialogBlock("DialogExample2", new List<Dialog>()
        {
            new Dialog("你好啊2", _loli),
            new Dialog("我叫小萝莉2", _loli),
        }),

    };
}

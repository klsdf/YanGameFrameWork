using System.Collections.Generic;
using UnityEngine;
using YanGameFrameWork.DialogSystem;
using TMPro;
using System;

public class DialogExample : MonoBehaviour
{


    public TMP_Text dialogText;

    public void Start()
    {
        // 注册所有对话块
        foreach (var dialogBlock in dialogBlocks)
        {
            DialogController.Instance.RegisterDialogBlock(dialogBlock);
        }
        
        DialogController.Instance.RunSequenceDialog("DialogExample", (dialog) =>
        {
            // dialogText.text = dialog.dialog;
            DialogController.Instance.StartTypingEffect(dialog.dialog, 0.1f, dialogText);
        }, () =>
        {
            Debug.Log("DialogExample end");
        });
    }


    static DialogCharacter Loli = new DialogCharacter("小萝莉");

    [NonSerialized]
    public List<DialogBlock> dialogBlocks = new List<DialogBlock>()
    {
        new DialogBlock("DialogExample", new List<Dialog>()
        {
            new Dialog("你好啊", Loli),
            new Dialog("我叫小萝莉", Loli),
            new Dialog("我最喜欢穿白丝", Loli),
            new Dialog("因为我是美味的小萝莉", Loli),
        }),
        new DialogBlock("DialogExample2", new List<Dialog>()
        {
            new Dialog("你好啊2", Loli),
            new Dialog("我叫小萝莉2", Loli),
        }),

    };
}

/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-29
 * Description: 对话控制器，主要用于控制对话块
 *
 * 修改记录：
 ****************************************************************************/

using YanGameFrameWork.Singleton;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DialogController : Singleton<DialogController>
{


    [SerializeField]
    private List<DialogBlock> _dialogBlocks = new List<DialogBlock>();

    public DialogBlock GetDialogBlockByName(string blockName)
    {
        return _dialogBlocks.Find(block => block.blockName == blockName);
    }

    public void RegisterDialogBlock(DialogBlock dialogBlock)
    {
        _dialogBlocks.Add(dialogBlock);
    }



}


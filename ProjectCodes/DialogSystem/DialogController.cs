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
using System.Collections;
using System;
using TMPro;

namespace YanGameFrameWork.DialogSystem
{
    public class DialogController : Singleton<DialogController>
    {


        [SerializeField]
        private List<DialogBlock> _dialogBlocks = new List<DialogBlock>();



        /// <summary>
        /// 根据对话块名称获取对话块
        /// </summary>
        /// <param name="blockName">对话块名称</param>
        /// <returns>对话块</returns>
        public DialogBlock GetDialogBlockByName(string blockName)
        {
            return _dialogBlocks.Find(block => block.blockName == blockName);
        }

        /// <summary>
        /// 注册对话块
        /// </summary>
        /// <param name="dialogBlock">对话块</param>
        public void RegisterDialogBlock(DialogBlock dialogBlock)
        {
            _dialogBlocks.Add(dialogBlock);
        }



        /// <summary>
        /// 运行顺序对话，每隔3秒调用一次GetNextDialog，直到对话结束。
        /// </summary>
        /// <param name="blockName">对话块名称</param>
        public void RunSequenceDialog(string blockName, Action<Dialog> onDialog, Action onDialogEnd)
        {
            StartCoroutine(RunDialogCoroutine(blockName, onDialog, onDialogEnd));
        }

        /// <summary>
        /// 协程：运行对话块中的对话，按下鼠标左键时进行下一句
        /// </summary>
        /// <param name="blockName">对话块名称</param>
        /// <param name="onDialog">对话回调</param>
        /// <returns>IEnumerator</returns>
        private IEnumerator RunDialogCoroutine(string blockName, Action<Dialog> onDialog, Action onDialogEnd)
        {
            DialogBlock dialogBlock = GetDialogBlockByName(blockName);
            if (dialogBlock == null)
            {
                YanGF.Debug.LogError(nameof(DialogController), $"对话块 '{blockName}' 未找到");
                yield break;
            }

            while (!dialogBlock.IsPlayEnd)
            {
                // 等待鼠标左键按下
                while (!Input.GetMouseButtonDown(0))
                {
                    yield return null;
                }

                Dialog dialog = dialogBlock.GetNextDialog();
                if (dialog != null)
                {
                    onDialog?.Invoke(dialog);
                }

                // 添加短暂延迟防止连续点击
                yield return new WaitForSeconds(0.1f);
            }

            YanGF.Debug.LogWarning(nameof(DialogController), $"对话块 '{blockName}' 已播放完毕");
            onDialogEnd?.Invoke();
        }

        /// <summary>
        /// 打字机效果：逐字显示文本
        /// </summary>
        /// <param name="dialog">要显示的对话</param>
        /// <param name="typingSpeed">打字速度，字符之间的延迟时间</param>
        public void StartTypingEffect(Dialog dialog, float typingSpeed, TMP_Text targetText)
        {
            StartCoroutine(TypeText(dialog, typingSpeed, targetText));
        }

        /// <summary>
        /// 协程：逐字显示文本
        /// </summary>
        /// <param name="dialog">要显示的对话</param>
        /// <param name="typingSpeed">打字速度，字符之间的延迟时间</param>
        /// <returns>IEnumerator</returns>
        private IEnumerator TypeText(Dialog dialog, float typingSpeed, TMP_Text targetText)
        {
            targetText.text = "";

            foreach (char letter in dialog.dialog.ToCharArray())
            {
                targetText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

    }
}

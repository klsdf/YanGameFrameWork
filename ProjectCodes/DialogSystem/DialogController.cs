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
using UnityEngine.UI;

namespace YanGameFrameWork.DialogSystem
{
    public class DialogController : Singleton<DialogController>
    {


        [SerializeField]
        private List<DialogBlock> _dialogBlocks = new List<DialogBlock>();


        [SerializeField]
        private bool _isTyping = false;


		[Header("Skip Settings")]
		[SerializeField]
		private Image _skipHoldImage; // 绑定的UI图片，用于显示长按进度

		[SerializeField]
		private float _skipHoldDuration = 1f; // 长按多久（秒）跳过当前block


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
        /// 运行顺序对话，每次鼠标点击会播放下一句，直到对话结束。
        /// </summary>
        /// <param name="blockName">对话块名称</param>
        /// <param name="onDialog">每一句对话的回调</param>
        /// <param name="onDialogEnd">当当前对话块播放完毕后，调用此回调</param>
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


            //先说一句
            Dialog dialog = dialogBlock.GetNextDialog();
            if (dialog != null)
            {
                onDialog?.Invoke(dialog);
            }

			// 初始化跳过UI
			if (_skipHoldImage != null)
			{
				_skipHoldImage.fillAmount = 0f;
				_skipHoldImage.gameObject.SetActive(false);
			}
			float skipHoldTimer = 0f;
			bool skippedBlock = false;

            //然后等待输入
            while (!dialogBlock.IsPlayEnd)
            {
				// 等待下一步：点击/提交 或 长按Ctrl跳过
				while (true)
				{
					bool submitPressed = Input.GetMouseButtonDown(0) || Input.GetButtonDown("Submit");
					bool ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

					// 长按Ctrl：累计时间并显示进度
					if (ctrlHeld)
					{
						skipHoldTimer += Time.deltaTime;
						if (_skipHoldImage != null)
						{
							_skipHoldImage.gameObject.SetActive(true);
							_skipHoldImage.fillAmount = Mathf.Clamp01(skipHoldTimer / Mathf.Max(0.01f, _skipHoldDuration));
						}

						if (skipHoldTimer >= _skipHoldDuration)
						{
							// 快进到当前block结束
							while (!dialogBlock.IsPlayEnd)
							{
								dialogBlock.GetNextDialog();
								yield return null;
							}
							skippedBlock = true;
							break;
						}
					}
					else
					{
						// 松开Ctrl：重置计时与UI
						if (_skipHoldImage != null)
						{
							_skipHoldImage.fillAmount = 0f;
							_skipHoldImage.gameObject.SetActive(false);
						}
						skipHoldTimer = 0f;
					}

					// 普通前进（不在打字中才响应）
					if (submitPressed && !_isTyping)
					{
						break;
					}

					yield return null;
				}

				if (skippedBlock)
				{
					break; // 跳出当前block
				}

                dialog = dialogBlock.GetNextDialog();
                if (dialog != null)
                {
                    onDialog?.Invoke(dialog);
                }

                // 添加短暂延迟防止连续点击
                yield return new WaitForSeconds(0.1f);
            }

			// 结束时隐藏跳过UI
			if (_skipHoldImage != null)
			{
				_skipHoldImage.fillAmount = 0f;
				_skipHoldImage.gameObject.SetActive(false);
			}

			YanGF.Debug.LogWarning(nameof(DialogController), $"对话块 '{blockName}' 已播放完毕");
            onDialogEnd?.Invoke();
        }

        /// <summary>
        /// 打字机效果：逐字显示文本
        /// </summary>
        /// <param name="dialog">要显示的对话</param>
        /// <param name="typingSpeed">打字速度，字符之间的延迟时间</param>
        /// <param name="targetText">目标文本组件</param>
        public void StartTypingEffect(string dialogText, float typingSpeed, TMP_Text targetText)
        {
            StartCoroutine(TypeText(dialogText, typingSpeed, targetText));
        }

        /// <summary>
        /// 协程：逐字显示文本
        /// </summary>
        /// <param name="dialog">要显示的对话</param>
        /// <param name="typingSpeed">打字速度，字符之间的延迟时间</param>
        /// <returns>IEnumerator</returns>
        private IEnumerator TypeText(string dialogText, float typingSpeed, TMP_Text targetText)
        {
            targetText.text = "";
            _isTyping = true;
            yield return null;

            foreach (char letter in dialogText.ToCharArray())
            {
                targetText.text += letter;

                // 在每次等待之前检查鼠标左键点击
                for (float timer = 0; timer < typingSpeed; timer += Time.deltaTime)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        // 如果点击，立即显示完整文本
                        targetText.text = dialogText;
                        yield return null;
                        _isTyping = false;
                        yield break;
                    }
                    yield return null;
                }
            }

            _isTyping = false;
        }

    }
}

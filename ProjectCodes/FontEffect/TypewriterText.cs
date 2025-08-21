/*
 * Author:      #AUTHORNAME#
 * CreateTime:  #CREATETIME#
 */
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 打字机文本效果组件：在初始化或调用时，按设定速度逐字显示 TMP 文本。
/// 独立于其它文字特效脚本存在，可与顶点动画类效果（如波浪、抖动）叠加使用。
/// 设计原因：将“初始显隐/过渡”与“持续顶点动画”解耦，降低耦合度，增强复用性。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(TMP_Text))]
public class TypewriterText : MonoBehaviour, ITypewriterControl
{
	/// <summary>
	/// 文本组件引用。
	/// </summary>
	private TMP_Text _text;

	/// <summary>
	/// 是否启用打字机效果。为 false 时直接显示全部文本。
	/// </summary>
	[SerializeField]
	public bool enableTypewriter = true;

	/// <summary>
	/// 是否在组件启用时自动播放。
	/// </summary>
	[SerializeField]
	public bool playOnEnable = true;

	/// <summary>
	/// 每秒显示的字符数（速度）。
	/// </summary>
	[SerializeField]
	public float charactersPerSecond = 30f;

	/// <summary>
	/// 是否启用全局渐显（从全透明到原始不透明度）。
	/// 设计原因：满足“初始化先全透明，然后渐渐显示”的需求，且与打字机并行进行。
	/// </summary>
	[SerializeField]
	public bool enableFadeIn = true;

	/// <summary>
	/// 渐显时长（秒）。
	/// </summary>
	[SerializeField]
	public float fadeInDuration = 0.5f;

	/// <summary>
	/// 开始播放前的延迟（秒）。
	/// </summary>
	[SerializeField]
	public float startDelay = 0f;

	/// <summary>
	/// 打字完成时的回调事件。
	/// </summary>
	public UnityEvent onCompleted = new UnityEvent();

	/// <summary>
	/// 当前运行中的打字协程。
	/// </summary>
	private Coroutine _playingRoutine;

	/// <summary>
	/// 播放开始前缓存的文本颜色（用于在播放完成或中断时恢复目标透明度）。
	/// </summary>
	private Color _cachedColorBeforePlay;

	/// <summary>
	/// 是否已缓存颜色。
	/// </summary>
	private bool _hasCachedColor;

	/// <summary>
	/// 目标可见字符总数（缓存）。
	/// </summary>
	private int _targetCharacterCount;

	/// <summary>
	/// 立即开始播放打字机效果（会从 0 重新计数）。
	/// </summary>
	public void Play()
	{
		EnsureTextRef();
		if (!enableTypewriter)
		{
			ShowAllImmediate();
			return;
		}
		RestartRoutine();
	}

	/// <summary>
	/// 停止播放并立即显示全部文本。
	/// </summary>
	public void StopAndShowAll()
	{
		EnsureTextRef();
		if (_playingRoutine != null)
		{
			StopCoroutine(_playingRoutine);
			_playingRoutine = null;
		}
		ShowAllImmediate();
		// 恢复为可见：如有缓存则恢复到原始透明度，否则强制为 1
		SetTextAlpha(_hasCachedColor ? _cachedColorBeforePlay.a : 1f);
	}

	/// <summary>
	/// 设置是否启用打字机效果（不立即触发播放）。
	/// </summary>
	/// <param name="enabled">是否启用</param>
	public void SetEnabled(bool enabled)
	{
		enableTypewriter = enabled;
	}

	/// <summary>
	/// 设置是否在组件启用时自动播放。
	/// </summary>
	/// <param name="enabled">是否在启用时自动播放</param>
	public void SetPlayOnEnable(bool enabled)
	{
		playOnEnable = enabled;
	}

	private void OnEnable()
	{
		EnsureTextRef();
		if (enableTypewriter && playOnEnable)
		{
			_text.ForceMeshUpdate();
			_text.maxVisibleCharacters = 0;
			RestartRoutine();
		}
		else
		{
			ShowAllImmediate();
		}
	}

	private void OnDisable()
	{
		if (_playingRoutine != null)
		{
			StopCoroutine(_playingRoutine);
			_playingRoutine = null;
		}
	}

	/// <summary>
	/// 协程：按照速度逐步增加可见字符数。
	/// </summary>
	private IEnumerator PlayRoutine()
	{
		// 缓存当前颜色并将初始透明度设为 0（若开启渐显）
		_cachedColorBeforePlay = _text.color;
		_hasCachedColor = true;
		float targetAlpha = _cachedColorBeforePlay.a;
		if (enableFadeIn)
		{
			SetTextAlpha(0f);
		}

		if (startDelay > 0f)
			yield return new WaitForSeconds(startDelay);

		_text.ForceMeshUpdate();
		_targetCharacterCount = _text.textInfo.characterCount;
		_text.maxVisibleCharacters = 0;

		float visibleCountFloat = 0f;
		float fadeElapsed = 0f;
		while (_text != null && _text.maxVisibleCharacters < _targetCharacterCount)
		{
			if (!enableTypewriter)
			{
				ShowAllImmediate();
				// 没有打字机时，若仍需渐显则直接设置到目标透明度
				if (enableFadeIn)
					SetTextAlpha(targetAlpha);
				yield break;
			}

			visibleCountFloat += charactersPerSecond * Time.deltaTime;
			int newCount = Mathf.Clamp(Mathf.FloorToInt(visibleCountFloat), 0, _targetCharacterCount);

			if (newCount != _text.maxVisibleCharacters)
			{
				_text.maxVisibleCharacters = newCount;
				_text.ForceMeshUpdate();
			}

			// 渐显（从 0 到目标透明度）
			if (enableFadeIn)
			{
				fadeElapsed += Time.deltaTime;
				float t = fadeInDuration > 0f ? Mathf.Clamp01(fadeElapsed / fadeInDuration) : 1f;
				float currentAlpha = Mathf.Lerp(0f, targetAlpha, t);
				SetTextAlpha(currentAlpha);
			}

			yield return null;
		}

		// 收尾：确保目标透明度与全部可见
		SetTextAlpha(targetAlpha);
		onCompleted?.Invoke();
		_playingRoutine = null;
	}

	/// <summary>
	/// 确保已缓存 TMP_Text 引用。
	/// </summary>
	private void EnsureTextRef()
	{
		if (_text == null)
			_text = GetComponent<TMP_Text>();
	}

	/// <summary>
	/// 重启播放协程。
	/// </summary>
	private void RestartRoutine()
	{
		if (_playingRoutine != null)
			StopCoroutine(_playingRoutine);
		_playingRoutine = StartCoroutine(PlayRoutine());
	}

	/// <summary>
	/// 立即显示全部字符。
	/// </summary>
	private void ShowAllImmediate()
	{
		_text.ForceMeshUpdate();
		_text.maxVisibleCharacters = _text.textInfo.characterCount;
		// 若未在播放流程中缓存颜色，则尽量保证全部可见
		if (!_hasCachedColor && enableFadeIn)
		{
			SetTextAlpha(1f);
		}
	}

	/// <summary>
	/// 设置文本整体透明度（保持原始 RGB）。
	/// </summary>
	/// <param name="alpha">目标透明度 0-1</param>
	private void SetTextAlpha(float alpha)
	{
		var col = _text.color;
		col.a = Mathf.Clamp01(alpha);
		_text.color = col;
	}
}



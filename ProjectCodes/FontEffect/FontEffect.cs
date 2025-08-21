/*
 * Author:      #AUTHORNAME#
 * CreateTime:  #CREATETIME#
 */
using UnityEngine;
using TMPro;

public enum FontEffectType
{
    Wave,
    Wiggle, // 修改 Wobble -> Wiggle 以匹配 Text Animator
    Bounce
}

/// <summary>
/// 打字机控制接口：用于与具体实现（如 TypewriterText）解耦。
/// 设计原因：避免在本文件直接依赖具体类，降低编译顺序与耦合风险。
/// </summary>
public interface ITypewriterControl
{
    /// <summary>
    /// 立即开始播放打字机效果。
    /// </summary>
    void Play();

    /// <summary>
    /// 停止播放并立刻显示全部字符。
    /// </summary>
    void StopAndShowAll();

    /// <summary>
    /// 设置是否启用打字机效果。
    /// </summary>
    /// <param name="enabled">是否启用</param>
    void SetEnabled(bool enabled);

    /// <summary>
    /// 设置组件启用时是否自动播放。
    /// </summary>
    /// <param name="enabled">是否自动播放</param>
    void SetPlayOnEnable(bool enabled);
}

public class FontEffect : MonoBehaviour
{
	/// <summary>
	/// TextMeshPro 组件引用。
	/// </summary>
    [Header("TextMeshPro 组件")]
    private TMP_Text _textMesh;
    private Mesh _mesh;
    private Vector3[] _vertices;

    [Header("效果类型")]
    public FontEffectType fontEffectType = FontEffectType.Bounce;

	// ===== 打字机初始显示控制 =====
	[Header("初始显示 - 打字机")]
	/// <summary>
	/// 是否在初始化时使用打字机效果显示文本。
	/// </summary>
	[SerializeField]
	private bool _enableTypewriterOnStart = false;

	/// <summary>
	/// 打字机效果组件引用；未赋值且启用时会在同对象上自动查找/添加。
	/// 设计原因：将“初始显隐/过渡”从顶点动画中解耦，按需组合。
	/// </summary>
	[SerializeField]
	private MonoBehaviour _typewriterBehaviour; // 以 MonoBehaviour 保存，避免因编译顺序导致找不到类型的报错

    // ===== Wave 效果参数 =====
    [Header("Wave 参数 (对应 <wave a=... f=... w=...>)")]
    [Header("振幅 (a) - 控制波浪的垂直幅度")]
    public float waveAmplitude = 18f;      // 对应 <wave a=1>
    [Header("频率 (f) - 控制波浪的波动速度")]
    public float waveFrequency = 1f;       // 对应 <wave f=1>
    [Header("波长 (w) - 控制波浪的水平拉伸")]
    [Range(0.01f, 1f)]
    public float waveWavelength = 0.2f;    // 对应 <wave w=0.1>

    // ===== Wiggle 效果参数 =====
    [Header("Wiggle 参数 (对应 <wiggle a=... f=...>)")]
    [Header("振幅 (a) - 控制抖动的幅度")]
    public float wiggleAmplitude = 1f;     // 对应 <wiggle a=1>
    [Header("频率 (f) - 控制抖动的速度")]
    public float wiggleFrequency = 1f;     // 对应 <wiggle f=1>


  // ===== Bounce 参数 ===== [新增]
    [Header("Bounce 参数 (对应 <bounce a=... f=... w=...>)")]
    [Tooltip("振幅 (a) - 弹跳高度")]
    public float bounceAmplitude = 2f;     // 对应 <bounce a=2>
    [Tooltip("频率 (f) - 弹跳速度")]
    public float bounceFrequency = 1f;     // 对应 <bounce f=1>
    [Tooltip("波长 (w) - 字符间弹跳相位差")]
    public float bounceWavelength = 0.2f;  // 对应 <bounce w=0.2>


    void Start()
    {
        _textMesh = GetComponent<TMP_Text>();
        _textMesh.RegisterDirtyVerticesCallback(OnTextMeshUpdated);

		// 初始化时按需启动打字机效果
		if (_enableTypewriterOnStart)
		{
			var typewriter = GetTypewriter();
			if (typewriter != null)
			{
				// 由本脚本主动触发，避免 OnEnable 与此处重复；同时确保已启用打字机
				typewriter.SetEnabled(true);
				typewriter.SetPlayOnEnable(false);
				typewriter.Play();
			}
		}
    }

    void OnDestroy()
    {
        _textMesh.UnregisterDirtyVerticesCallback(OnTextMeshUpdated);
    }

    private void OnTextMeshUpdated()
    {
        ApplyFontEffect();
    }

    void Update()
    {
        ApplyFontEffect();
    }

    private void ApplyFontEffect()
    {
        _textMesh.ForceMeshUpdate();
        _mesh = _textMesh.mesh;
        _vertices = _mesh.vertices;

        for (int i = 0; i < _textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = _textMesh.textInfo.characterInfo[i];
			// 跳过不可见字符（与打字机 maxVisibleCharacters 兼容）
			if (!c.isVisible) continue;
            int index = c.vertexIndex;

            Vector3 offset = Vector3.zero;
            switch (fontEffectType)
            {
                case FontEffectType.Wave:
                    offset = WaveEffect(Time.time, i);
                    break;
                case FontEffectType.Wiggle:
                    offset = WiggleEffect(Time.time, i);
                    break;
                case FontEffectType.Bounce:
                    offset = BounceEffect(Time.time, i);
                    break;
            }

            // 应用偏移到字符的 4 个顶点
            for (int j = 0; j < 4; j++)
            {
                _vertices[index + j] += offset;
            }
        }

        _mesh.vertices = _vertices;
        _textMesh.canvasRenderer.SetMesh(_mesh);
    }

	/// <summary>
	/// 触发一次打字机显示；若无组件则自动添加。
	/// 适用场景：在运行时脚本化启动初始打字显示。
	/// </summary>
	public void PlayTypewriter()
	{
		var typewriter = GetOrAddTypewriter();
		if (typewriter == null) return;
		typewriter.SetEnabled(true);
		typewriter.SetPlayOnEnable(false);
		typewriter.Play();
	}

	/// <summary>
	/// 停止打字机并立刻显示全部。
	/// </summary>
	public void StopTypewriterAndShowAll()
	{
		var typewriter = GetTypewriter();
		if (typewriter == null) return;
		typewriter.StopAndShowAll();
	}

	/// <summary>
	/// 运行时开关打字机启用状态（不自动开始）。
	/// </summary>
	/// <param name="enabled">是否启用</param>
	public void SetTypewriterEnabled(bool enabled)
	{
		var typewriter = GetOrAddTypewriter();
		if (typewriter != null)
			typewriter.SetEnabled(enabled);
	}

	/// <summary>
	/// 获取打字机组件（若 Inspector 绑定了其它同类脚本也可），找不到则返回 null。
	/// 使用接口样式的访问，避免在本类顶部直接引用具体类型导致的编译顺序问题。
	/// </summary>
	private ITypewriterControl GetTypewriter()
	{
		if (_typewriterBehaviour != null && _typewriterBehaviour is ITypewriterControl cached)
			return cached;
		return GetComponent<ITypewriterControl>();
	}

	/// <summary>
	/// 获取或添加打字机组件，并缓存到 _typewriterBehaviour。
	/// </summary>
	private ITypewriterControl GetOrAddTypewriter()
	{
		var tw = GetTypewriter();
		if (tw == null)
		{
			// 未找到实现 ITypewriterControl 的组件：交由外部显式添加，避免直接依赖具体实现
			Debug.LogWarning("未找到打字机组件(ITypewriterControl)。请在对象上添加 TypewriterText 或自定义实现。");
			return null;
		}
		_typewriterBehaviour = tw as MonoBehaviour;
		return tw;
	}

    /// <summary>
    /// Wave 效果（类似正弦波浪）
    /// </summary>
    /// <param name="time">当前时间</param>
    /// <param name="charIndex">字符索引（用于相位偏移）</param>
    Vector3 WaveEffect(float time, int charIndex)
    {
        float y = Mathf.Sin(time * waveFrequency + charIndex * waveWavelength) * waveAmplitude;
        return new Vector3(0, y, 0);
    }

    /// <summary>
    /// Wiggle 效果（随机抖动）
    /// </summary>
    /// <param name="time">当前时间</param>
    /// <param name="charIndex">字符索引（用于随机种子）</param>
    Vector3 WiggleEffect(float time, int charIndex)
    {
        // 使用 Perlin 噪声实现平滑随机抖动
        float x = Mathf.PerlinNoise(charIndex * 0.1f, time * wiggleFrequency) * 2 - 1;
        float y = Mathf.PerlinNoise(charIndex * 0.1f + 100, time * wiggleFrequency) * 2 - 1;
        return new Vector3(x, y, 0) * wiggleAmplitude;
    }


     /// <summary>
    /// Bounce 效果（垂直弹跳，落地后暂停）
    /// </summary>
    /// <param name="time">当前时间</param>
    /// <param name="charIndex">字符索引（用于相位偏移）</param>
    Vector3 BounceEffect(float time, int charIndex)
    {
        // 使用正弦函数模拟弹跳效果，并在落地时暂停
        float phase = time * bounceFrequency + charIndex * bounceWavelength;
        float y = Mathf.Abs(Mathf.Sin(phase)) * bounceAmplitude;

        // 在落地时暂停
        if (Mathf.Sin(phase) < 0.1f && Mathf.Sin(phase) > -0.1f)
        {
            y = 0; // 暂停时将 y 设置为 0
        }

        return new Vector3(0, y, 0);
    }
}
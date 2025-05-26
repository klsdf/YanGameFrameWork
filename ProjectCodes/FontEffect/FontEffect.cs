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

public class FontEffect : MonoBehaviour
{
    [Header("TextMeshPro 组件")]
    private TMP_Text textMesh;
    private Mesh mesh;
    private Vector3[] vertices;

    [Header("效果类型")]
    public FontEffectType fontEffectType = FontEffectType.Bounce;

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
        textMesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
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
                vertices[index + j] += offset;
            }
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
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
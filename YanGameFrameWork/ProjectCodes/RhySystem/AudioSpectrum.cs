/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-08
 *
 * Description: 根据音频的响度来生成音符
 下面是一些具体的实现细节：
 1. 基本思路是实时获得音频的各个频谱，然后计算RMS。来得到实时的响度。
 2. 然后根据RMS和当前的响度做比较，得到音频的重音部分。
 3. 要注意的是得到的响度并不精确，因为音乐中存在高潮，和低潮。高潮会导致，一段音乐全部都是重音，所以需要使用滑动窗口来平滑响度。
 而低潮会导致整体的RMS会拉下去，所以需要设计静音阈值，来剔除响度较低的音符。
****************************************************************************/

using UnityEngine;

public class AudioLoudness : MonoBehaviour
{
    public AudioSource audioSource;
    public int numSamples = 1024;
    public RectTransform uiElement;
    public float maxHeight = 200f;
    public float sensitivity = 1.5f; // 灵敏度系数
    public GameObject notePrefab; // 音符预制件
    public Transform noteSpawnPoint; // 音符生成位置
    private float[] samples;
    private float[] windowSamples;
    private int windowSize = 256; // 滑动窗口大小
    private int windowIndex = 0;
    public float silenceThreshold = 0.01f; // 静音阈值

    void Start()
    {
        samples = new float[numSamples];
        windowSamples = new float[windowSize];

        // 从AudioClip中计算初始窗口值
        if (audioSource.clip != null)
        {
            float[] clipData = new float[audioSource.clip.samples * audioSource.clip.channels];
            audioSource.clip.GetData(clipData, 0);
            float initialWindowValue = CalculateRMS(clipData);

            // 初始化滑动窗口
            for (int i = 0; i < windowSize; i++)
            {
                windowSamples[i] = initialWindowValue;
            }
        }
    }

    void Update()
    {
        audioSource.GetOutputData(samples, 0);
        float currentLoudness = CalculateRMS(samples);

        // 更新滑动窗口
        windowSamples[windowIndex] = currentLoudness;
        windowIndex = (windowIndex + 1) % windowSize;

        // 计算动态阈值
        float dynamicThreshold = CalculateRMS(windowSamples) * sensitivity;

        // 比较当前响度和动态阈值
        if (currentLoudness > dynamicThreshold)
        {
            Debug.Log("重音检测: " + currentLoudness);
            CreateNote();
        }

        // 根据响度调整UI元素的高度
        float newHeight = Mathf.Clamp(currentLoudness * maxHeight, 0, maxHeight);
        uiElement.sizeDelta = new Vector2(uiElement.sizeDelta.x, newHeight);
    }

    void CreateNote()
    {
        GameObject note = Instantiate(notePrefab, noteSpawnPoint.position, Quaternion.identity);
    }

    float CalculateRMS(float[] samples)
    {
        // float sum = 0f;
        // for (int i = 0; i < samples.Length; i++)
        // {
        //     sum += samples[i] * samples[i];
        // }
        // return Mathf.Sqrt(sum / samples.Length);
        float sum = 0f;
        int count = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            if (Mathf.Abs(samples[i]) > silenceThreshold)
            {
                sum += samples[i] * samples[i];
                count++;
            }
        }
        return count > 0 ? Mathf.Sqrt(sum / count) : 0f;

    }
}

/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-11
 *
 * Description: 根据BPM和节拍生成信号
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum RhythmType
{
    Strong,//强
    Weak,//弱
    Middle,//次强
}


public class RhythmGenerator : MonoBehaviour
{
    [Header("BPM")]
    public float bpm = 120f; // 每分钟节拍数

    [Header("初始暂停时间")]
    public float initialPauseTime = 2f; // 初始暂停时间

    private int _beatCounter = 0; // 节拍计数器
    private float _timer = 0f; // 时间累加器
    private float _rhythmInterval; // 节拍间隔

    private bool _hasStarted = false; // 是否已开始生成节拍信号

    private void Start()
    {
        // 根据BPM计算节奏间隔时间
        _rhythmInterval = 60f / bpm;
    }

    private void Update()
    {
        // 累加时间
        _timer += Time.deltaTime;

        RhythmType rhythmType = RhythmType.Strong;
        // 检查是否超过初始暂停时间
        if (!_hasStarted && _timer >= initialPauseTime)
        {
            _hasStarted = true;
            _timer = 0f; // 重置时间累加器以开始节拍
        }

        // 只有在暂停时间结束后才开始生成节拍信号
        if (_hasStarted && _timer >= _rhythmInterval)
        {
            // 根据节拍计数器输出信号
            switch (_beatCounter)
            {
                case 0:
                    rhythmType = RhythmType.Strong;
                    break;
                case 1:
                    rhythmType = RhythmType.Weak;
                    break;
                case 2:
                    rhythmType = RhythmType.Middle;
                    break;
                case 3:
                    rhythmType = RhythmType.Weak;
                    break;
            }

            // 更新节拍计数器
            _beatCounter = (_beatCounter + 1) % 4;

            // 重置时间累加器
            _timer -= _rhythmInterval;

            YanGF.Event.TriggerEvent<RhythmType>(RhythmEvent.OnRhythm, rhythmType);
        }
    }
}

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

    private int _beatCounter = 0; // 节拍计数器（0-3 对应 4/4 拍：强-弱-中-弱）
    private float _timer = 0f; // 时间累加器（用于整拍）
    private float _halfTimer = 0f; // 半拍计时器
    private float _rhythmInterval; // 节拍间隔（整拍）
    private float _halfInterval; // 半拍间隔

    private bool _hasStarted = false; // 是否已开始生成节拍信号

    private void Start()
    {
        // 根据BPM计算节奏间隔时间
        _rhythmInterval = 60f / bpm;
        _halfInterval = _rhythmInterval * 0.5f;
    }

    private void Update()
    {
        // 累加时间
        float deltaTime = Time.deltaTime;
        _timer += deltaTime;
        _halfTimer += deltaTime;

        RhythmType rhythmType = RhythmType.Strong;
        // 检查是否超过初始暂停时间
        if (!_hasStarted && _timer >= initialPauseTime)
        {
            _hasStarted = true;
            _timer = 0f; // 重置时间累加器以开始节拍
            _halfTimer = 0f; // 重置半拍计时器
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

        // 半拍：在整拍之间的中点触发，不区分强弱
        if (_hasStarted && _halfTimer >= _halfInterval)
        {
            _halfTimer -= _halfInterval;
            YanGF.Event.TriggerEvent(RhythmEvent.OnHalfRhythm);
        }
    }
}

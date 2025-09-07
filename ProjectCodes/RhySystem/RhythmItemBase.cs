/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-11
 *
 * Description: 节奏基类，用于接受节奏信号
 */

using UnityEngine;


public abstract class RhythmItemBase : MonoBehaviour
{

    /// <summary>是否响应强拍</summary>
    public bool checkStrong = true;
    /// <summary>是否响应次强拍</summary>
    public bool checkMiddle = true;
    /// <summary>是否响应弱拍</summary>
    public bool checkWeak = true;
    /// <summary>是否响应半拍</summary>
    public bool checkHalf = false;

    void Start()
    {
        YanGF.Event.AddListener<RhythmType>(RhythmEvent.OnRhythm, OnRhythm);
        YanGF.Event.AddListener(RhythmEvent.OnHalfRhythm, OnHalfRhythm);
    }

    private void OnDestroy()
    {
        // 反注册，避免对象销毁后仍然接收事件导致空引用
        YanGF.Event.RemoveListener<RhythmType>(RhythmEvent.OnRhythm, OnRhythm);
        YanGF.Event.RemoveListener(RhythmEvent.OnHalfRhythm, OnHalfRhythm);
    }

    /// <summary>
    /// 当收到节奏信号时
    /// </summary>
    /// <param name="rhythmType"></param>
    private void OnRhythm(RhythmType rhythmType)
    {
        if (rhythmType == RhythmType.Strong && checkStrong)
        {
            OnRhythmEvent();
        }
        if (rhythmType == RhythmType.Middle && checkMiddle)
        {
            OnRhythmEvent();
        }
        if (rhythmType == RhythmType.Weak && checkWeak)
        {
            OnRhythmEvent();
        }
    }

    /// <summary>
    /// 收到半拍事件时的回调
    /// </summary>
    private void OnHalfRhythm()
    {
        if (checkHalf)
        {
            OnRhythmEvent();
        }
    }


    
    public abstract void OnRhythmEvent();


}




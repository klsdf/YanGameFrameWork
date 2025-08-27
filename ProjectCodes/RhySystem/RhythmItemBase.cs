/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-11
 *
 * Description: 节奏基类，用于接受节奏信号
 */

using UnityEngine;


public abstract class RhythmItemBase : MonoBehaviour
{

    public bool checkStrong = true;
    public bool checkMiddle = true;
    public bool checkWeak = true;

    void Start()
    {
        YanGF.Event.AddListener<RhythmType>(RhythmEvent.OnRhythm, OnRhythm);
    }

    /// <summary>
    /// 当收到节奏信号时
    /// </summary>
    /// <param name="rhythmType"></param>
    private void OnRhythm(RhythmType rhythmType)
    {
        if (rhythmType == RhythmType.Strong && checkStrong)
        {

            OnBeat();
            OnStrongBeat();
        }
        if (rhythmType == RhythmType.Middle && checkMiddle)
        {
            OnMiddleBeat();
        }
        if (rhythmType == RhythmType.Weak && checkWeak)
        {
            OnWeakBeat();
        }
    }

    /// <summary>
    /// 不管节拍的强弱，反正是拍子就触发
    /// </summary>
    /// <param name="rhythmType"></param>
    public abstract void OnBeat();
    
    public abstract void OnStrongBeat();
    public abstract void OnMiddleBeat();
    public abstract void OnWeakBeat();


}




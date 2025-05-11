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

    private  void OnRhythm(RhythmType rhythmType){
        if(rhythmType == RhythmType.Strong && checkStrong){
            OnBeat(RhythmType.Strong);
        }
        if(rhythmType == RhythmType.Middle && checkMiddle){
            OnBeat(RhythmType.Middle);
        }
        if(rhythmType == RhythmType.Weak && checkWeak){
            OnBeat(RhythmType.Weak);
        }
    }

    public abstract void OnBeat(RhythmType rhythmType);


}




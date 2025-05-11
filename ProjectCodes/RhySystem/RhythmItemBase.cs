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
        if (checkStrong)
        {
            YanGF.Event.AddListener<RhythmType>(RhythmType.Strong.ToString(), OnRhythm);
        }
        if (checkMiddle)
        {
            YanGF.Event.AddListener<RhythmType>(RhythmType.Middle.ToString(), OnRhythm);
        }
        if (checkWeak)
        {
            YanGF.Event.AddListener<RhythmType>(RhythmType.Weak.ToString(), OnRhythm);
        }
    }

    public abstract void OnRhythm(RhythmType rhythmType);
}




using System;
using UnityEngine;
/// <summary>
/// 等待命令
/// </summary>
public class WaitCommand : ICommand
{
    public enum WaitType
    {
        Seconds,
        Frames,
        Custom
    }

    private readonly WaitType _type;
    private readonly float _value;
    private readonly Func<bool> _customCondition;
    private bool _isCompleted;
    private float _elapsedTime;
    private int _elapsedFrames;

    public bool IsCompleted => _isCompleted;

    public WaitCommand(WaitType type, float value = 0, Func<bool> customCondition = null)
    {
        _type = type;
        _value = value;
        _customCondition = customCondition;
        _isCompleted = false;
    }

    public void Execute()
    {
        switch (_type)
        {
            case WaitType.Seconds:
                _elapsedTime += Time.deltaTime;
                _isCompleted = _elapsedTime >= _value;
                break;
            case WaitType.Frames:
                _elapsedFrames++;
                _isCompleted = _elapsedFrames >= _value;
                break;
            case WaitType.Custom:
                _isCompleted = _customCondition?.Invoke() ?? true;
                break;
        }
    }

    public void Stop()
    {
        _isCompleted = true;
    }
}

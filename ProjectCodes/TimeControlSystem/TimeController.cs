using System;
using System.Collections.Generic;
using YanGameFrameWork.CoreCodes;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 基础命令接口
/// </summary>
public interface ICommand
{
    void Execute();
    void Stop();
    bool IsCompleted { get; }
}

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

/// <summary>
/// 执行命令
/// </summary>
public class ExecuteCommand : ICommand
{
    private readonly Action _action;
    private readonly bool _isRepeatable;
    private bool _isCompleted;

    public bool IsCompleted => _isCompleted;

    public ExecuteCommand(Action action, bool isRepeatable = false)
    {
        _action = action;
        _isRepeatable = isRepeatable;
        _isCompleted = false;
    }

    public void Execute()
    {
        _action?.Invoke();
        if (!_isRepeatable)
        {
            _isCompleted = true;
        }
    }

    public void Stop()
    {
        _isCompleted = true;
    }
}

/// <summary>
/// 序列命令
/// </summary>
public class SequenceCommand : ICommand
{
    private readonly List<ICommand> _commands = new List<ICommand>();
    private int _currentIndex;
    private bool _isCompleted;

    public bool IsCompleted => _isCompleted;

    public void AddCommand(ICommand command)
    {
        _commands.Add(command);
    }

    public void Execute()
    {
        if (_isCompleted || _currentIndex >= _commands.Count)
            return;

        var currentCommand = _commands[_currentIndex];
        currentCommand.Execute();

        if (currentCommand.IsCompleted)
        {
            _currentIndex++;
            if (_currentIndex >= _commands.Count)
            {
                _isCompleted = true;
            }
        }
    }

    public void Stop()
    {
        foreach (var command in _commands)
        {
            command.Stop();
        }
        _isCompleted = true;
    }
}



/// <summary>
/// 命令执行器
/// </summary>
public class CommandExecutor : MonoBehaviour
{
    private readonly List<ICommand> _activeCommands = new List<ICommand>();

    private void Update()
    {
        for (int i = _activeCommands.Count - 1; i >= 0; i--)
        {
            var command = _activeCommands[i];
            command.Execute();

            if (command.IsCompleted)
            {
                _activeCommands.RemoveAt(i);
            }
        }
    }

    public void ExecuteCommand(ICommand command)
    {
        _activeCommands.Add(command);
    }

    public void StopAllCommands()
    {
        foreach (var command in _activeCommands)
        {
            command.Stop();
        }
        _activeCommands.Clear();
    }
}



public class TimeController : Singleton<TimeController>
{
    private CommandExecutor _executor;

    protected override void Awake()
    {
        base.Awake();
        _executor = gameObject.AddComponent<CommandExecutor>();
    }

    /// <summary>
    /// 延迟执行某个动作
    /// </summary>
    public void SetTimeOut(Action action, float seconds)
    {
        var sequence = new SequenceCommand();
        sequence.AddCommand(new WaitCommand(WaitCommand.WaitType.Seconds, seconds));
        sequence.AddCommand(new ExecuteCommand(action));
        _executor.ExecuteCommand(sequence);
    }

    /// <summary>
    /// 定期重复执行某个动作
    /// </summary>
    public void SetInterval(Action action, float seconds)
    {
        var sequence = new SequenceCommand();
        sequence.AddCommand(new ExecuteCommand(action));
        sequence.AddCommand(new WaitCommand(WaitCommand.WaitType.Seconds, seconds));
        sequence.AddCommand(new ExecuteCommand(() => SetInterval(action, seconds)));
        _executor.ExecuteCommand(sequence);
    }

    /// <summary>
    /// 按帧等待
    /// </summary>
    public void WaitFrames(Action action, int frames)
    {
        var sequence = new SequenceCommand();
        sequence.AddCommand(new WaitCommand(WaitCommand.WaitType.Frames, frames));
        sequence.AddCommand(new ExecuteCommand(action));
        _executor.ExecuteCommand(sequence);
    }

    /// <summary>
    /// 等待自定义条件
    /// </summary>
    public void WaitUntil(Action action, Func<bool> condition)
    {
        var sequence = new SequenceCommand();
        sequence.AddCommand(new WaitCommand(WaitCommand.WaitType.Custom, customCondition: condition));
        sequence.AddCommand(new ExecuteCommand(action));
        _executor.ExecuteCommand(sequence);
    }

    /// <summary>
    /// 停止所有命令
    /// </summary>
    public void StopAll()
    {
        _executor.StopAllCommands();
    }

    [Button("测试")]
    public void Test()
    {
        // 测试延时执行
        SetTimeOut(() => Debug.Log("1秒后执行"), 1);

        // 测试间隔执行
        SetInterval(() => Debug.Log("每秒执行一次"), 1);

        // 测试按帧等待
        WaitFrames(() => Debug.Log("等待10帧后执行"), 10);

        // 测试自定义条件
        float startTime = Time.time;
        WaitUntil(
            () => Debug.Log("等待3秒后执行"),
            () => Time.time - startTime >= 3
        );

        // 5秒后停止所有命令
        SetTimeOut(() => StopAll(), 5);
    }
}

/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-29
 * Description: 时间控制器，主要包括跟js同款的setTimeout、setInterval方法。
 以及函数调用限制器CreateRateLimitedAction
 ****************************************************************************/

using System;
using System.Collections.Generic;
using YanGameFrameWork.Singleton;
using UnityEngine;
using YanGameFrameWork.Editor;
using System.Collections;



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





namespace YanGameFrameWork.TimeControlSystem
{
    public class TimeController : Singleton<TimeController>
    {
        private CommandExecutor _executor;

        protected void Awake()
        {
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





        /// <summary>
        /// 创建一个频率限制的委托。
        /// </summary>
        /// <param name="action">需要限制频率的动作。</param>
        /// <param name="interval">限制的时间间隔（秒）。</param>
        /// <returns>包装后的委托。</returns>
        public Action CreateRateLimitedAction(Action action, float interval)
        {
            bool canInvoke = true;

            return () =>
            {
                if (!canInvoke)
                    return;

                canInvoke = false;
                action.Invoke();
                // 使用协程来重置 canInvoke 标志
                StartCoroutine(ResetInvokeFlag(interval, () => canInvoke = true));
            };
        }

        private IEnumerator ResetInvokeFlag(float interval, Action resetAction)
        {
            yield return new WaitForSeconds(interval);
            resetAction.Invoke();
        }

    }
}
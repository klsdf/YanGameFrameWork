using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class CoroutineTaskRunner
{
    public Task Task => _taskCompletionSource.Task;
    private TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();

    private MonoBehaviour _host;
    private Coroutine _coroutine;
    private IEnumerator _routine;

    private bool _isPaused = false;
    private bool _isStopped = false;

    public Action OnStarted { get; set; }
    public Action OnCompleted { get; set; }

    private CoroutineTaskRunner(IEnumerator routine)
    {
        _host = GlobalCoroutineHost.Instance;
        _routine = RoutineWrapper(routine);
        _coroutine = _host.StartCoroutine(_routine);
    }

    public static CoroutineTaskRunner Run(IEnumerator routine)
    {
        return new CoroutineTaskRunner(routine);
    }

    private IEnumerator RoutineWrapper(IEnumerator original)
    {
        bool started = false;
        while (!_isStopped && original.MoveNext())
        {
            if (!started)
            {
                started = true;
                OnStarted?.Invoke();
            }
            while (_isPaused)
                yield return null;
            yield return original.Current;
        }
        OnCompleted?.Invoke();
        _taskCompletionSource.TrySetResult(true);
    }

    public void Pause() => _isPaused = true;
    public void Resume() => _isPaused = false;

    public void Stop()
    {
        _isStopped = true;
        if (_coroutine != null)
            _host.StopCoroutine(_coroutine);
        OnCompleted?.Invoke();

        //本来打算用TrySetCanceled来抛出异常，但是仔细一想，中断协程抛出异常未免太神秘，还是用TrySetResult正常返回吧
        // try
        // {
        //     _taskCompletionSource.TrySetCanceled();
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError(e);
        // }

        _taskCompletionSource.TrySetResult(true);
    }

    public System.Runtime.CompilerServices.TaskAwaiter GetAwaiter()
    {
        return Task.GetAwaiter();
    }
}

using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    CoroutineTaskRunner runner;

    async void Start()
    {
        Debug.Log("开始执行任务序列");

        await CoroutineTaskRunner.Run(TaskA());
        await CoroutineTaskRunner.Run(TaskB());
        await CoroutineTaskRunner.Run(TaskC());
        await StartAsync();

        Debug.Log("所有任务完成了喵~");
    }


    IEnumerator MyRoutine()
    {
        while (true)
        {
            Debug.Log($"协程运行中");
            yield return null;
        }
    }

    async Task StartAsync()
    {
        runner = CoroutineTaskRunner.Run(MyRoutine());
        await runner;
        Debug.Log("协程完成了喵~");
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            runner?.Pause();
        if (Input.GetKeyDown(KeyCode.R))
            runner?.Resume();
        if (Input.GetKeyDown(KeyCode.S))
            runner?.Stop();
    }


    IEnumerator TaskA()
    {
        Debug.Log("A 开始");
        yield return new WaitForSeconds(1);
        Debug.Log("A 结束");
    }

    IEnumerator TaskB()
    {
        Debug.Log("B 开始");
        yield return new WaitForSeconds(2);
        Debug.Log("B 结束");
    }

    IEnumerator TaskC()
    {
        Debug.Log("C 开始");
        yield return new WaitForSeconds(1);
        Debug.Log("C 结束");
    }
}

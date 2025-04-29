public class ExampleTimerUse : MonoBehaviour
{
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

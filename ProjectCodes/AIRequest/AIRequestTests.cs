using System;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
/// <summary>
/// 这个类用于演示如何使用AIRequest类。
/// </summary>
public class AIRequestExample : MonoBehaviour
{
    /// <summary>
    /// 应用程序的入口点。
    /// </summary>
    ///
    // [Button("测试")]
    public async void Test()
    {
        // 创建AIRequest的实例
        AIRequest aiRequest = new AIRequest();

        try
        {
            // 调用SendRequestAsync方法并获取响应
            string response = await aiRequest.SendRequestAsync(
                new AIRequestBody.Message[] {
                    new AIRequestBody.Message { role = "system", content = "你是一个白发红瞳美少女，你喜欢我。" },
                    new AIRequestBody.Message { role = "user", content = "你好啊（摸摸头）" }
                }
            );

            // 解析响应为AIResponse对象
            AIResponse aiResponse = JsonConvert.DeserializeObject<AIResponse>(response);

            // 打印每个Choice的详细信息
            foreach (var choice in aiResponse.Choices)
            {
                Debug.Log($"索引: {choice.Index}, 消息: {choice.Message.Content}, 完成原因: {choice.FinishReason}");
            }
        }
        catch (Exception ex)
        {
            // 处理可能的异常
            Debug.LogError("请求失败: " + ex.Message);
        }
    }
}



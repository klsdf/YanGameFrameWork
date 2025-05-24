using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 这个类用于处理AI请求。
/// </summary>
public class AIRequest
{
    /// <summary>
    /// DeepSeek API的URL。
    /// </summary>
    private const string ApiUrl = "https://api.deepseek.com/chat/completions";

    /// <summary>
    /// DeepSeek API的密钥。
    /// </summary>
    private static string ApiKey
    {
        get
        {
            // 从加密文件中加载并解密API密钥
            string filePath = Application.dataPath + "/StreamingAssets/api_key.enc";
            string password = "114514"; // 使用实际的密码
            return ApiKeyManager.LoadAndDecryptApiKey(filePath, password);
        }
    }

    /// <summary>
    /// 发送请求到DeepSeek API。
    /// </summary>
    /// <returns>返回API的响应。</returns>
    public async Task<string> SendRequestAsync()
    {
        // 创建请求体对象
        var requestBody = new AIRequestBody
        {
            model = "deepseek-chat",
            messages = new[]
            {
                new AIRequestBody.Message { role = "system", content = "你是一个白发红瞳美少女，你喜欢我。" },
                new AIRequestBody.Message { role = "user", content = "你好啊（摸摸头）" }
            },
            stream = false
        };

        // 将请求体序列化为JSON字符串
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

        // 创建HttpRequester实例
        var requester = new HttpRequester(ApiUrl)
            .SetMethod(HTTPMethod.POST)
            .AddHeader("Authorization", $"Bearer {ApiKey}")
            .SetContentType(ContentType.Application_Json)
            .SetRequestBodyWithJson(json);

        // 执行请求
        await requester.ExecuteAsync();

        // 检查响应是否成功
        if (requester.IsResponseSuccessful())
        {
            // 获取响应内容
            return requester.GetResponseContent();
        }
        else
        {
            throw new Exception("请求失败");
        }
    }
}
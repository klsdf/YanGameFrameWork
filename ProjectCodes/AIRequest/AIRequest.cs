using System;
using System.Net.Http;
using System.Text;
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
        using (HttpClient client = new HttpClient())
        {
            // 设置请求头
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

            // 请求体
            var requestBody = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new { role = "system", content = "你是一个白发红瞳美少女，你喜欢我。" },
                    new { role = "user", content = "你好啊（摸摸头）" }
                },
                stream = false
            };

            // 将请求体序列化为JSON字符串
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            // 发送POST请求，设置Content-Type为application/json
            HttpResponseMessage response = await client.PostAsync(ApiUrl, new StringContent(json, Encoding.UTF8, "application/json"));

            // 确保请求成功
            response.EnsureSuccessStatusCode();

            // 读取响应内容
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
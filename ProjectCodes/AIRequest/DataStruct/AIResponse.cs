using Newtonsoft.Json;
public class AIResponse
{
    /// <summary>
    /// 响应的唯一标识符。
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// 响应的对象类型。
    /// </summary>
    [JsonProperty("object")]
    public string Object { get; set; }

    /// <summary>
    /// 响应创建的时间戳。
    /// </summary>
    [JsonProperty("created")]
    public long Created { get; set; }

    /// <summary>
    /// 使用的模型。
    /// </summary>
    [JsonProperty("model")]
    public string Model { get; set; }

    /// <summary>
    /// 选择的列表。
    /// </summary>
    [JsonProperty("choices")]
    public Choice[] Choices { get; set; }

    /// <summary>
    /// 使用情况。
    /// </summary>
    [JsonProperty("usage")]
    public Usage Usage { get; set; }
}

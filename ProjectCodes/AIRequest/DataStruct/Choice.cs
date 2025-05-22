using Newtonsoft.Json;
public class Choice
{
    /// <summary>
    /// 选择的索引。
    /// </summary>
    [JsonProperty("index")]
    public int Index { get; set; }

    /// <summary>
    /// 消息。
    /// </summary>
    [JsonProperty("message")]
    public Message Message { get; set; }

    /// <summary>
    /// 完成的原因。
    /// </summary>
    [JsonProperty("finish_reason")]
    public string FinishReason { get; set; }
}


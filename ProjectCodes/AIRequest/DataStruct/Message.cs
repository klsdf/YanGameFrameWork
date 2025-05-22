using Newtonsoft.Json;
public class Message
{
    /// <summary>
    /// 消息的角色。
    /// </summary>
    [JsonProperty("role")]
    public string Role { get; set; }

    /// <summary>
    /// 消息的内容。
    /// </summary>
    [JsonProperty("content")]
    public string Content { get; set; }
}


using Newtonsoft.Json;
public class Usage
{
    /// <summary>
    /// 提示的令牌数。
    /// </summary>
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    /// 完成的令牌数。
    /// </summary>
    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }

    /// <summary>
    /// 总令牌数。
    /// </summary>
    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }
}

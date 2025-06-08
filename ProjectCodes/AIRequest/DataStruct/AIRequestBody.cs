// 定义请求体的数据结构
public class AIRequestBody
{
    public string model { get; set; }
    public Message[] messages { get; set; }
    public bool stream { get; set; }
    /// <summary>
    /// 响应格式。
    /// </summary>
    public object response_format { get; set; }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}

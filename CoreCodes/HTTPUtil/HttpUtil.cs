using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// HTTP请求工具类，用于发送GET和POST请求
/// </summary>
public static class HttpUtil
{
    
    public enum HTTPMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    
   public static void Get(string url, Action<string> onSuccess, Action<string> onError)
    {

    }
}

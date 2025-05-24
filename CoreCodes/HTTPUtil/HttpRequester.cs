using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum HTTPMethod
{
    GET,
    POST,
    PUT,
    DELETE
}

public enum ContentType
{
    Application_Json,
    Multipart_Form_Data,
}

/// <summary>
/// HTTP请求工具类，用于发送GET和POST请求
/// </summary>
public class HttpRequester
{
    private string _baseUrl;
    private HTTPMethod _method;
    private Dictionary<string, string> _headers = new Dictionary<string, string>();
    private Dictionary<string, string> _queryParameters = new Dictionary<string, string>();

    private string _responseContent;
    private HttpContent _requestContent;
    private HttpResponseMessage _response; // 用于存储HTTP响应

    public HttpRequester(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public HttpRequester SetMethod(HTTPMethod method)
    {
        _method = method;
        return this;
    }

    /// <summary>
    /// 添加请求头
    /// </summary>
    /// <param name="key">头的键</param>
    /// <param name="value">头的值</param>
    /// <returns>返回HttpRequester对象</returns>
    public HttpRequester AddHeader(string key, string value)
    {
        _headers[key] = value;
        return this;
    }




    public HttpRequester SetContentType(ContentType contentType)
    {
        switch (contentType)
        {
            case ContentType.Application_Json:
                _headers["Content-Type"] = "application/json";
                break;
            case ContentType.Multipart_Form_Data:
                _headers["Content-Type"] = "multipart/form-data";
                break;
        }
        return this;
    }

    /// <summary>
    /// 添加URL查询参数
    /// </summary>
    public HttpRequester AddQueryParameter(string name, string value)
    {
        _queryParameters[name] = value;
        return this;
    }


    /// <summary>
    /// 设置请求体
    /// </summary>
    /// <param name="body">请求体的json字符串</param>
    /// <returns>返回HttpRequester对象</returns>
    public HttpRequester SetRequestBodyWithJson(string body)
    {
        _requestContent = new StringContent(body, Encoding.UTF8, "application/json");
        return this;
    }


    public HttpRequester InitMultipartFormData()
    {
        _requestContent = new MultipartFormDataContent();
        return this;
    }

    public HttpRequester AddString(string name, string value)
    {
        (_requestContent as MultipartFormDataContent).Add(new StringContent(value), name);
        return this;
    }

    public HttpRequester AddFile(string fieldName, string filePath)
    {
        var fileBytes = File.ReadAllBytes(filePath);
        var fileName = Path.GetFileName(filePath);
        (_requestContent as MultipartFormDataContent).Add(new ByteArrayContent(fileBytes), fieldName, fileName);
        return this;
    }



    // /// <summary>
    // /// 设置请求的内容
    // /// </summary>
    // /// <param name="content">请求的内容</param>
    // /// <returns>返回HttpRequester对象</returns>
    // public HttpRequester SetContent(HttpContent content)
    // {
    //     if (_method == HTTPMethod.POST || _method == HTTPMethod.PUT)
    //     {
    //         // 设置请求的内容
    //         _requestContent = content; // 使用新的请求内容
    //     }
    //     return this;
    // }

    // /// <summary>
    // /// 添加文件到请求中
    // /// </summary>
    // /// <param name="fileName">文件名称</param>
    // /// <param name="filePath">文件路径</param>
    // /// <returns>返回HttpRequester对象</returns>
    // public HttpRequester AddFile(string fileName, string filePath)
    // {
    //     if (!File.Exists(filePath))
    //     {
    //         Debug.LogError("文件未找到: " + filePath);
    //         throw new FileNotFoundException("文件未找到", filePath);
    //     }

    //     var fileBytes = File.ReadAllBytes(filePath);
    //     if (fileBytes == null || fileBytes.Length == 0)
    //     {
    //         Debug.LogError("无法读取文件内容");
    //         throw new InvalidOperationException("无法读取文件内容");
    //     }

    //     if (_requestContent is MultipartFormDataContent multipartContent)
    //     {
    //         multipartContent.Add(new ByteArrayContent(fileBytes), "file", fileName);
    //     }
    //     else
    //     {
    //         multipartContent = new MultipartFormDataContent();
    //         multipartContent.Add(new ByteArrayContent(fileBytes), "file", fileName);
    //         SetContent(multipartContent);
    //     }

    //     return this;
    // }

    /// <summary>
    /// 执行HTTP请求并返回HttpRequester对象以支持链式调用
    /// </summary>
    /// <returns>返回HttpRequester对象</returns>
    public async Task<HttpRequester> ExecuteAsync()
    {
        using (var client = new HttpClient())
        {
            // 在发送请求之前，打印出所有请求参数
            foreach (var param in _queryParameters)
            {
                Debug.Log($"参数名: {param.Key}, 参数值: {param.Value}");
            }

            // 构建请求URL
            var url = _baseUrl;
            if (_method == HTTPMethod.GET && _queryParameters.Count > 0)
            {
                var query = new FormUrlEncodedContent(_queryParameters).ReadAsStringAsync().Result;
                url = $"{_baseUrl}?{query}";
            }

            // 创建请求消息
            var request = new HttpRequestMessage(new HttpMethod(_method.ToString()), url);

            // 添加请求头
            foreach (var header in _headers)
            {
                // 跳过Content-Type头，因为它应该通过HttpContent来设置
                if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                request.Headers.Add(header.Key, header.Value);
            }

            // 添加请求体参数
            if ((_method == HTTPMethod.POST || _method == HTTPMethod.PUT) && _requestContent != null)
            {
                request.Content = _requestContent;
            }
            else if (_method == HTTPMethod.POST || _method == HTTPMethod.PUT)
            {
                Debug.LogError("请求内容未正确初始化: 方法是 " + _method + "，但请求体未设置");
                throw new InvalidOperationException("请求内容未正确初始化");
            }

            // 发送请求并获取响应
            _response = await client.SendAsync(request);
            _responseContent = await _response.Content.ReadAsStringAsync();
        }
        return this;
    }

    /// <summary>
    /// 检查响应是否成功
    /// </summary>
    /// <returns>如果响应成功返回true，否则返回false</returns>
    public bool IsResponseSuccessful()
    {
        return _response != null && _response.IsSuccessStatusCode;
    }

    /// <summary>
    /// 获取响应内容
    /// </summary>
    /// <returns>响应内容</returns>
    public string GetResponseContent()
    {
        return _responseContent;
    }
}

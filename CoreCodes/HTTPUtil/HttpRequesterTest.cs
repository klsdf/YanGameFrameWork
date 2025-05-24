using System;
using System.Threading.Tasks;
using System.Net.Http;
using Sirenix.OdinInspector;
using UnityEngine;
public class HttpRequesterTest : MonoBehaviour
{
    // /// <summary>
    // /// 测试GET请求
    // /// </summary>
    // /// 
    // [Button("测试GET请求")]
    // public static async Task TestGetRequest(string url)
    // {
    //     var requester = new HttpRequester(url)
    //         .SetMethod(HTTPMethod.GET);

    //     await requester.ExecuteAsync();

    //     if (requester.IsResponseSuccessful())
    //     {
    //         Console.WriteLine("GET请求成功:");
    //         Console.WriteLine(requester.GetResponseContent());
    //     }
    //     else
    //     {
    //         Console.WriteLine("GET请求失败");
    //     }
    // }

    // /// <summary>
    // /// 测试POST请求
    // /// </summary>
    // [Button("测试POST请求")]
    // public static async Task TestPostRequest()
    // {
    //     var url = "https://jsonplaceholder.typicode.com/posts";
    //     var requester = new HttpRequester(url)
    //         .SetMethod(HTTPMethod.POST)
    //         .AddHeader("Content-Type", "application/json")
    //         .AddParameter("application/json", "{\"title\": \"foo\", \"body\": \"bar\", \"userId\": 1}", ParameterType.RequestBody);

    //     await requester.ExecuteAsync();

    //     if (requester.IsResponseSuccessful())
    //     {
    //         Console.WriteLine("POST请求成功:");
    //         Console.WriteLine(requester.GetResponseContent());
    //     }
    //     else
    //     {
    //         Console.WriteLine("POST请求失败");
    //     }
    // }
}

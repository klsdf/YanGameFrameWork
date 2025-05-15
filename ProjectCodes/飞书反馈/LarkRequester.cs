using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace YanGameFrameWork.LarkAdapter
{
    struct TableData
    {
        //https://meowjito.feishu.cn/base/BSuWbqnCBajojmsAcyCcE4jmnVg?table=tblgGRWK29092Uxa&view=vewhxWYUOe
        //也就是中间的BSuWbqnCBajojmsAcyCcE4jmnVg
        public string app_token;
        //也就是中间的tblgGRWK29092Uxa
        public string table_id;

        public TableData(string app_token, string table_id)
        {
            this.app_token = app_token;
            this.table_id = table_id;
        }
    }

    struct AppData
    {
        public string app_id;
        public string app_secret;

        public AppData(string app_id, string app_secret)
        {
            this.app_id = app_id;
            this.app_secret = app_secret;
        }
    }

    public struct TenantAccessTokenResponse
    {
        public int code;
        public string msg;
        public string tenant_access_token;
        public int expire;
    }


    /// <summary>
    /// 飞书请求的工具类
    /// </summary>
    public static  class LarkRequester
    {

    //多维表格的app_token和table_id
        static TableData _tableData = new TableData(
                app_token: "BSuWbqnCBajojmsAcyCcE4jmnVg",
                table_id: "tblgGRWK29092Uxa"
        );

        static AppData _appData = new AppData(
            app_id: "cli_a8ab2efd769a1013",
            app_secret: "uV7tVbBL2EUOsAQgzTmmlTMg2lXlJEES"
        );
        static LarkRequester()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "lark_config.enc");
            string password = "114514";
            var larkConfig = LarkConfigUtil.LoadConfig(path, password);
            _tableData = new TableData(larkConfig.app_token, larkConfig.table_id);
            Debug.Log("larkConfig: " + larkConfig.app_token + " " + larkConfig.table_id);
            _appData = new AppData(larkConfig.app_id, larkConfig.app_secret);
            Debug.Log("larkConfig: " + larkConfig.app_id + " " + larkConfig.app_secret);
        }


        //身份验证的token
        static string Bearer
        {
            get
            {
                return GetTenantAccessToken();
            }
        }






        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="recordBody">记录体</param>
        /// <returns>返回添加的记录</returns>
        public static string AddRecord(RecordBody recordBody)
        {
            var client = new RestClient($"https://open.feishu.cn/open-apis/bitable/v1/apps/{_tableData.app_token}/tables/{_tableData.table_id}/records");
            var request = new RestRequest();
            string body = JsonConvert.SerializeObject(recordBody);
            request.Method = Method.Post;
            request.AddHeader("Authorization", $"Bearer {Bearer}");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            request.Timeout = TimeSpan.FromSeconds(10);
            RestResponse response = client.Execute(request);
            return response.Content;
        }


        /// <summary>
        /// 上传图片到多维表格，注意path要带图片的后缀
        /// </summary>
        /// <param name="filePath">图片路径</param>
        /// <param name="fileName">图片名称</param>
        /// <param name="parentType">父类型</param>
        /// <param name="size">图片大小</param>
        /// <returns>返回图片的file_token</returns>
        public static string UploadImageAndGetFileToken(
            string filePath)
        {
            var client = new RestClient("https://open.feishu.cn/open-apis/drive/v1/medias/upload_all");
            var request = new RestRequest();
            request.Method = Method.Post;

            string parentType = "bitable_image";//多维表格的图片格式
            string fileName = Path.GetFileName(filePath);
            long size = new FileInfo(filePath).Length;

            request.AddHeader("Authorization", $"Bearer {Bearer}");
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddParameter("file_name", fileName);
            request.AddParameter("parent_type", parentType);
            request.AddParameter("parent_node", _tableData.app_token);
            request.AddParameter("size", size.ToString());
            // request.AddParameter("checksum", checksum);
            // request.AddParameter("extra", extraJson);
            request.AddFile("file", filePath);
            RestResponse response = client.Execute(request);


            //解析返回的json
            var json = JObject.Parse(response.Content);
            string fileToken = json["data"]?["file_token"]?.ToString();
            Debug.Log("file_token: " + fileToken);
            return fileToken;
        }

        /// <summary>
        /// 向多维表格新增字段（列）
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="type">字段类型（如1为文本，2为数字等，详见飞书文档）</param>
        /// <param name="appToken">可选，表格app_token</param>
        /// <param name="tableId">可选，表格table_id</param>
        /// <returns>接口响应内容</returns>
        public static string AddField(string fieldName, FieldType type)
        {
            var client = new RestClient(
                $"https://open.feishu.cn/open-apis/bitable/v1/apps/{_tableData.app_token}/tables/{_tableData.table_id}/fields");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {Bearer}");
            var body = JsonConvert.SerializeObject(new { field_name = fieldName, type = type });
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            return response.Content;
        }

        /// <summary>
        /// 删除多维表格字段（列）
        /// </summary>
        /// <param name="fieldId">字段ID</param>
        /// <returns>接口响应内容</returns>
        public static string DeleteFieldById(string fieldId)
        {
            var client = new RestClient($"https://open.feishu.cn/open-apis/bitable/v1/apps/{_tableData.app_token}/tables/{_tableData.table_id}/fields/{fieldId}");
            var request = new RestRequest();
            request.Method = Method.Delete;
            request.AddHeader("Authorization", $"Bearer {Bearer}");
            RestResponse response = client.Execute(request);
            return response.Content;
        }


        /// <summary>
        /// 删除多维表格字段（列）
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>接口响应内容</returns>
        public static string DeleteFieldByName(string fieldName)
        {
            var fieldId = GetFieldIdByName(fieldName);
            if (fieldId == null)
            {
                throw new Exception("字段不存在");
            }
            return DeleteFieldById(fieldId);
        }




        /// <summary>
        /// 获取所有字段列表
        /// </summary>
        /// <returns>字段信息列表</returns>
        public static List<FieldInfo> GetAllField()
        {
            var client = new RestClient($"https://open.feishu.cn/open-apis/bitable/v1/apps/{_tableData.app_token}/tables/{_tableData.table_id}/fields?page_size=20");
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Authorization", $"Bearer {Bearer}");
            RestResponse response = client.Execute(request);
            var respObj = JsonConvert.DeserializeObject<FieldListResponse>(response.Content);
            if (respObj != null && respObj.code == 0 && respObj.data != null)
            {
                return respObj.data.items;
            }

            Debug.LogError("获取字段列表失败: " + response.Content);
            return null;
        }

        /// <summary>
        /// 根据字段名获取字段ID
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns>字段ID，未找到返回null</returns>
        public static string GetFieldIdByName(string fieldName)
        {
            var fields = GetAllField();
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    if (field.field_name == fieldName)
                        return field.field_id;
                }
            }
            return null;
        }



        /// <summary>
        /// 同步字段
        /// </summary>
        /// <param name="tableStructConfig">字段配置</param>
        public static void SyncFieldsWithConfig(TableStructConfig tableStructConfig)
        {
            // 1. 获取当前表的所有字段
            var allFields = GetAllField();
            if (allFields == null)
            {
                Debug.LogError("获取表字段失败");
                return;
            }
            var allFieldNames = new HashSet<string>();
            foreach (var field in allFields)
                allFieldNames.Add(field.field_name);

            // 2. 配置字段名集合
            var configFieldNames = new HashSet<string>();
            foreach (var item in tableStructConfig.items)
                configFieldNames.Add(item.fieldName);

            // 3. 找出表格里有但配置没有的字段，删除
            foreach (var field in allFields)
            {
                if (!configFieldNames.Contains(field.field_name))
                {
                    try
                    {
                        DeleteFieldById(field.field_id);
                        Debug.Log($"已删除字段：{field.field_name}（ID: {field.field_id}）");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"删除字段 {field.field_name} 失败: {e.Message}");
                    }
                }
            }

            // 4. 找出配置有但表格没有的字段，创建
            foreach (var item in tableStructConfig.items)
            {
                if (!allFieldNames.Contains(item.fieldName))
                {
                    try
                    {
                        AddField(item.fieldName, item.fieldType);
                        Debug.Log($"已创建字段：{item.fieldName}（类型: {item.fieldType}）");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"创建字段 {item.fieldName} 失败: {e.Message}");
                    }
                }
            }
            Debug.Log("字段同步完成");
        }

        public static int GetAllRecordsCount()
        {
            var allRecords = GetAllRecords();
            return allRecords.Count;
        }

        public static List<JObject> GetAllRecords()
        {
            var allRecords = new List<JObject>();
            string pageToken = null;
            do
            {
                var client = new RestClient($"https://open.feishu.cn/open-apis/bitable/v1/apps/{_tableData.app_token}/tables/{_tableData.table_id}/records/search?page_size=20" + (pageToken != null ? $"&page_token={pageToken}" : ""));
                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Authorization", $"Bearer {Bearer}");
                request.AddHeader("Content-Type", "application/json");
                var body = "{}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                RestResponse response = client.Execute(request);

                var json = JObject.Parse(response.Content);
                var items = json["data"]?["items"] as JArray;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        allRecords.Add((JObject)item);
                    }
                }
                pageToken = json["data"]?["page_token"]?.ToString();
            } while (!string.IsNullOrEmpty(pageToken));

            return allRecords;
        }


        /// <summary>
        /// 获取tenant_access_token
        /// </summary>
        /// <param name="appId">飞书应用的app_id</param>
        /// <param name="appSecret">飞书应用的app_secret</param>
        /// <returns>tenant_access_token</returns>
        public static string GetTenantAccessToken()
        {
            var client = new RestClient("https://open.feishu.cn/open-apis/auth/v3/tenant_access_token/internal");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/json; charset=utf-8");

            string body = JsonConvert.SerializeObject(_appData);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            RestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var tokenResp = JsonConvert.DeserializeObject<TenantAccessTokenResponse>(response.Content);
                return tokenResp.tenant_access_token;
            }
            else
            {
                Debug.LogError("获取 tenant_access_token 失败: " + response.Content);
                return null;
            }
        }

    }

}
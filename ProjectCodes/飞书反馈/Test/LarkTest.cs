using UnityEngine;
using System.Collections.Generic;
using System;


namespace YanGameFrameWork.LarkAdapter
{


    public struct TableStructConfigItem
    {
        public string fieldName;
        public FieldType fieldType;

        public TableStructConfigItem(string fieldName, FieldType fieldType)
        {
            this.fieldName = fieldName;
            this.fieldType = fieldType;
        }
    }

    public struct TableStructConfig
    {
        public List<TableStructConfigItem> items;

        public TableStructConfig(List<TableStructConfigItem> items)
        {
            this.items = items;
        }


    }



    public class LarkTest : MonoBehaviour
    {
        // [Button("发送")]
        public async void Send(string feedbackContent)
        {
            string filePath = Application.dataPath + "/StreamingAssets/test.png";
            string fileToken = await LarkRequester.UploadImageAndGetFileTokenAsync(filePath);
            var recordBody = new RecordBody(new Fields(
                记录ID: 123,
                反馈内容: feedbackContent,
                创建时间: DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                截图: new List<Attachment> { new Attachment(fileToken) }
            ));
            string result = await LarkRequester.AddRecordAsync(recordBody);
            Debug.Log(result);
        }




        // [Button("发送图片测试")]
        public async void SendImageTest()
        {
            string filePath = Application.dataPath + "/StreamingAssets/test.png";
            string fileToken = await LarkRequester.UploadImageAndGetFileTokenAsync(filePath);
            Debug.Log(fileToken);
        }




        // [Button("新增字段")]
        public async void AddField(string fieldName)
        {
            string result = await LarkRequester.AddFieldAsync(fieldName, FieldType.Text);
            Debug.Log(result);
        }

        // [Button("获取所有字段")]
        public async void GetField()
        {
            var fields = await LarkRequester.GetAllFieldAsync();
            foreach (var field in fields)
            {
                Debug.Log(field.field_name);
            }
        }





        // [Button("删除字段")]
        public async void DeleteField(string fieldName)
        {
            string result = await LarkRequester.DeleteFieldByNameAsync(fieldName);
            Debug.Log(result);
        }


        // [Button("同步字段")]
        public async void SyncFields()
        {
            var tableStructConfig = new TableStructConfig(new List<TableStructConfigItem> {
                new TableStructConfigItem("记录ID", FieldType.Text),
                new TableStructConfigItem("反馈内容", FieldType.Text),
                new TableStructConfigItem("截图", FieldType.Attachment),
                new TableStructConfigItem("创建时间", FieldType.Date),
            });
            await LarkRequester.SyncFieldsWithConfigAsync(tableStructConfig);
        }

        // [Button("获取所有记录")]
        public async void GetAllRecordsTest()
        {
            var records = await LarkRequester.GetAllRecordsAsync();
            Debug.Log($"总记录数: {records.Count}");
            foreach (var record in records)
            {
                Debug.Log(record.ToString());
            }
        }

        // [Button("获取tenant_access_token")]
        public async void GetTenantAccessTokenTest()
        {
            string tenantAccessToken = await LarkRequester.GetTenantAccessTokenAsync();
            Debug.Log(tenantAccessToken);
        }

    }
}


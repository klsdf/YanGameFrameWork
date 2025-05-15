using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;


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
        [Button("发送")]
        public void Send()
        {
            string filePath = Application.dataPath + "/StreamingAssets/test.png";
            string fileToken = LarkRequester.UploadImageAndGetFileToken(filePath);
            var recordBody = new RecordBody(new Fields(
                记录ID: 123,
                反馈内容: "好评",
                创建时间: DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                截图: new List<Attachment> { new Attachment(fileToken) }
            ));
            string result = LarkRequester.AddRecord(recordBody);
            Debug.Log(result);
        }


        [Button("新增字段")]
        public void AddField()
        {
            string result = LarkRequester.AddField("测试文本", FieldType.Text);
            Debug.Log(result);
        }

        [Button("获取所有字段")]
        public void GetField()
        {
            var fields = LarkRequester.GetAllField();
            foreach (var field in fields)
            {
                Debug.Log(field.field_name);
            }
        }





        [Button("删除字段")]
        public void DeleteField()
        {
            string result = LarkRequester.DeleteFieldByName("测试文本");
            Debug.Log(result);
        }


        [Button("同步字段")]
        public void SyncFields()
        {
            var tableStructConfig = new TableStructConfig(new List<TableStructConfigItem> {
                new TableStructConfigItem("记录ID", FieldType.Text),
                new TableStructConfigItem("反馈内容", FieldType.Text),
                new TableStructConfigItem("截图", FieldType.Attachment),
                new TableStructConfigItem("创建时间", FieldType.Date),
            });
            LarkRequester.SyncFieldsWithConfig(tableStructConfig);
        }

        [Button("获取所有记录")]
        public void GetAllRecordsTest()
        {
            var records = LarkRequester.GetAllRecords();
            Debug.Log($"总记录数: {records.Count}");
            foreach (var record in records)
            {
                Debug.Log(record.ToString());
            }
        }

        [Button("获取tenant_access_token")]
        public void GetTenantAccessTokenTest()
        {
            string tenantAccessToken = LarkRequester.GetTenantAccessToken();
            Debug.Log(tenantAccessToken);
        }

    }
}


using System.Collections.Generic;
using System;

namespace YanGameFrameWork.LarkAdapter
{


    // 飞书多维表格字段类型枚举
    public enum FieldType
    {
        Text = 1,           // 文本
        Number = 2,         // 数字
        SingleSelect = 3,   // 单选
        MultiSelect = 4,    // 多选
        Date = 5,           // 日期
        Checkbox = 7,       // 复选框
        Person = 11,        // 人员
        Phone = 13,         // 电话
        Url = 15,           // 超链接
        Attachment = 17,    // 附件
        SingleLink = 18,    // 单向关联
        Formula = 20,       // 公式
        TwoWayLink = 21,    // 双向关联
        Location = 22,      // 地理位置
        Group = 23,         // 分组
        CreateTime = 1001,  // 创建时间
        UpdateTime = 1002,  // 修改时间
        Creator = 1003,     // 创建人
        Modifier = 1004,    // 修改人
        AutoNumber = 1005   // 自动编号
    }


    [Serializable]
    public struct Fields
    {
        public long 记录ID;
        public string 反馈内容;
        public long 创建时间;
        public List<Attachment> 截图;

        public Fields(long 记录ID, string 反馈内容, long 创建时间, List<Attachment> 截图)
        {
            this.记录ID = 记录ID;
            this.反馈内容 = 反馈内容;
            this.创建时间 = 创建时间;
            this.截图 = 截图;
        }
    }

    [Serializable]
    public struct RecordBody
    {
        public Fields fields;

        public RecordBody(Fields fields)
        {
            this.fields = fields;
        }
    }

    [Serializable]
    public struct Attachment
    {
        public string file_token;

        public Attachment(string file_token)
        {
            this.file_token = file_token;
        }
    }

    // 字段信息结构体
    public class FieldInfo
    {
        public string field_id;
        public string field_name;
        public bool is_primary;
        public object property;
        public int type;
        public string ui_type;
    }


    // 字段列表返回结构体
    public class FieldListResponse
    {
        public int code;
        public FieldListData data;
        public string msg;
    }
    public class FieldListData
    {
        public bool has_more;
        public List<FieldInfo> items;
        public string page_token;
        public int total;
    }

}


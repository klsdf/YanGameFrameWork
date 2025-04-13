using System;
namespace YanGameFrameWork.SaveSystem
{
    // 单条数据存储结构
    [Serializable]
    public class SaveData
    {
        public string Key;
        public string JsonData; // 以 JSON 形式存储数据，保证泛型支持
    }

}
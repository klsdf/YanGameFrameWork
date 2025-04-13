using System;
using System.Collections.Generic;
using UnityEngine;

namespace YanGameFrameWork.SaveSystem
{
    // 用于存储所有数据的列表
    [Serializable]
    public class SaveDataList
    {
        public List<SaveData> DataList = new List<SaveData>();
    }
}
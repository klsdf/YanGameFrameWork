using System.Collections.Generic;

/// <summary>
/// DebugItem桶类，用于存储和管理相同日志信息的DebugItem实例。
/// </summary>
public class DebugItemBucket
{
    // 使用字典存储日志信息和对应的DebugItem列表
    private Dictionary<string, List<DebugItem>> itemBuckets = new Dictionary<string, List<DebugItem>>();

    /// <summary>
    /// 添加一个DebugItem到桶中。
    /// </summary>
    /// <param name="logString">日志信息</param>
    /// <param name="debugItem">DebugItem实例</param>
    public void AddItem(string logString, DebugItem debugItem)
    {
        if (!itemBuckets.ContainsKey(logString))
        {
            itemBuckets[logString] = new List<DebugItem>();
        }
        itemBuckets[logString].Add(debugItem);
    }

    /// <summary>
    /// 获取指定日志信息的DebugItem列表。
    /// </summary>
    /// <param name="logString">日志信息</param>
    /// <returns>DebugItem列表</returns>
    public List<DebugItem> GetItems(string logString)
    {
        if (itemBuckets.ContainsKey(logString))
        {
            return itemBuckets[logString];
        }
        return new List<DebugItem>();
    }

    public List<string> GetAllLogStrings()
    {
        return new List<string>(itemBuckets.Keys);
    }

    public void Clear()
    {
        itemBuckets.Clear();
    }
} 
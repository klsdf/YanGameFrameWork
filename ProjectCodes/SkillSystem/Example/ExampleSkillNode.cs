using UnityEngine;



public class ExampleSkillNode : SkillNode   
{
    /// <summary>
    /// 当解锁了该节点后，会自动调用这个函数，用来存储解锁后的数据
    /// </summary>
    public override void Save()
    {
        // 在这里实现保存逻辑
        // 例如：保存到PlayerPrefs、数据库等
        Debug.Log($"技能节点 {nodeData.Name} 已解锁并保存");
    }

    /// <summary>
    /// 当技能节点被锁定时，显示的信息
    /// </summary>
    protected override void ShowLockInfo()
    {
        // 实现锁定状态的显示逻辑
        // 例如：改变按钮颜色、显示锁定图标等
        Debug.Log($"技能节点 {nodeData.Name} 处于锁定状态");
    }

    /// <summary>
    /// 当技能节点可以解锁时，显示的信息
    /// </summary>
    protected override void ShowCanUnlockInfo()
    {
        // 实现可解锁状态的显示逻辑
        // 例如：改变按钮颜色、显示解锁提示等
        Debug.Log($"技能节点 {nodeData.Name} 可以解锁");
    }

    /// <summary>
    /// 当技能节点被解锁时，显示的信息
    /// </summary>
    protected override void ShowUnlockInfo()
    {
        // 实现已解锁状态的显示逻辑
        // 例如：改变按钮颜色、显示已解锁图标等
        Debug.Log($"技能节点 {nodeData.Name} 已解锁");
    }
}
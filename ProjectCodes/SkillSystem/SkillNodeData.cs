/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-15
 * Description: 技能节点数据
 *
 ****************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子类需要重写构造函数，来初始化自己的数据
/// </summary>
public abstract class SkillNodeData
{
    public string Name { get;  set; }
    public string Description { get;  set; }


    public bool HasUnlocked { get;set; }


    public SkillNodeData Parent { get; set; }
    public List<SkillNodeData> Children { get;  set; }

    /// <summary>
    /// 这个技能节点所属的技能系统
    /// </summary>
    public SkillSystem SkillSystem { get; set; }


    /// <summary>
    /// 解锁这个技能的条件
    /// </summary>
    public Func<bool> Condition;

    /// <summary>
    /// 当解锁时执行的事件
    /// </summary>
    public Action<GameObject> OnUnlock;

    /// <summary>
    /// 技能数据挂载的实际对象
    /// </summary>
    public GameObject skillObject;


    /// <summary>
    /// 技能节点数据
    /// </summary>
    /// <param name="name">技能名称</param>
    /// <param name="description">技能描述</param>
    /// <param name="onUnlock">技能解锁事件</param>
    public SkillNodeData(string name, string description, Action<GameObject> onUnlock, Func<bool> condition, SkillSystem skillSystem)
    {
        Name = name;
        Description = description;
        HasUnlocked = false;
        Children = new List<SkillNodeData>();
        OnUnlock = onUnlock;
        Condition = condition;
        SkillSystem = skillSystem;
    }



    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="child">返回添加的节点</param>
    /// <returns></returns>
    public SkillNodeData AddChild(SkillNodeData child)
    {
        Children.Add(child);
        child.Parent = this;
        return child;
    }
}


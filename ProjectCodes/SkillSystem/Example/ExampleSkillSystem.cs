using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class ExampleSkillSystem : SkillSystem
{
    public class ExampleSkillData : SkillNodeData
    {
        public ExampleSkillData(string name, string description, Action<GameObject> onUnlock, Func<bool> condition, SkillSystem skillSystem)
            : base(name, description, onUnlock, condition, skillSystem)
        {
            // 其他初始化代码
        }
    }

    public int SkillPoint = 20;

    public override List<SkillNodeData> CreateInitData()
    {
        SkillNodeData _rootSkill = InitNode(new ExampleSkillData("攻击增强1", "攻击力+1", (gameObject) =>
        {
            SkillPoint -= 1;

            YanGF.Debug.Log(nameof(ExampleSkillSystem), "攻击增强1，消耗1点技能点");
            gameObject.GetComponent<Image>().color = Color.green;
        }, () =>
        {
            return SkillPoint >= 1;
        }, this));

        SkillNodeData node1 = InitNode(new ExampleSkillData("攻击增强2", "攻击力+2", (gameObject) =>
        {
            SkillPoint -= 1;

            YanGF.Debug.Log(nameof(ExampleSkillSystem), "攻击增强2，消耗1点技能点");
            gameObject.GetComponent<Image>().color = Color.green;
        }, () =>
        {
            return SkillPoint >= 1;
        }, this));

        SkillNodeData node2 = InitNode(new ExampleSkillData("攻击增强3", "攻击力+3", (gameObject) =>
        {
            SkillPoint -= 1;

            YanGF.Debug.Log(nameof(ExampleSkillSystem), "攻击增强3，消耗1点技能点");
            gameObject.GetComponent<Image>().color = Color.green;
        }, () =>
        {
            return SkillPoint >= 1;
        }, this));

        SkillNodeData node3 = InitNode(new ExampleSkillData("防御增强1", "防御力+1", (gameObject) =>
        {
            SkillPoint -= 1;

            YanGF.Debug.Log(nameof(ExampleSkillSystem), "防御增强1，消耗1点技能点");
            gameObject.GetComponent<Image>().color = Color.green;
        }, () =>
        {
            return SkillPoint >= 1;
        }, this));

        _rootSkill
        .AddChild(node1)
        .AddChild(node2);

        node1.AddChild(node3);

        return new List<SkillNodeData> { _rootSkill };
    }

    protected override List<string> LoadUnlockedSkillName()
    {
        List<string> unlockedSkillNames = YanGF.Save.Load("UnlockedSkills", new List<string>(), "ExampleSkillSystem");
        return unlockedSkillNames;
    }


}

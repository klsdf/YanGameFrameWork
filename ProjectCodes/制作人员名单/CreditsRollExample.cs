using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 制作人员名单使用示例
/// 展示如何使用新的制作人员名单框架
/// </summary>
public class CreditsRollExample : MonoBehaviour
{
    [Header("制作人员名单控制器")]
    public CreditsRoll creditsRoll;
    
    [Header("示例数据")]
    [SerializeField]
    private List<CreditsRollData> exampleCredits = new List<CreditsRollData>();
    
    private void Start()
    {
        if (creditsRoll == null)
        {
            creditsRoll = FindObjectOfType<CreditsRoll>();
        }
        
        SetupExampleCredits();
    }
    
    /// <summary>
    /// 设置示例制作人员名单
    /// </summary>
    private void SetupExampleCredits()
    {
        exampleCredits = new List<CreditsRollData>
        {
            // 标题
            new CreditsTitleData("游戏制作团队", Color.yellow, 40, 4f),
            new CreditsDividerData(40f, Color.gray, 1.5f),
            
            // 核心团队
            new CreditsTextData("核心团队", Color.cyan, 32, 2f),
            new CreditsDividerData(20f, Color.gray, 1f),
            
            new CreditsAvatarData("张三", "项目总监", "Avatars/Director", null, 4f),
            new CreditsAvatarData("李四", "主程序", "Avatars/LeadProgrammer", null, 4f),
            new CreditsAvatarData("王五", "主美术", "Avatars/LeadArtist", null, 4f),
            new CreditsAvatarData("赵六", "主策划", "Avatars/LeadDesigner", null, 4f),
            
            new CreditsDividerData(30f, Color.gray, 1.5f),
            
            // 程序团队
            new CreditsTextData("程序团队", Color.cyan, 28, 2f),
            new CreditsDividerData(15f, Color.gray, 1f),
            
            new CreditsTextData("客户端程序：小明", Color.white, 24, 2f),
            new CreditsTextData("服务器程序：小红", Color.white, 24, 2f),
            new CreditsTextData("工具程序：小刚", Color.white, 24, 2f),
            
            new CreditsDividerData(25f, Color.gray, 1.5f),
            
            // 美术团队
            new CreditsTextData("美术团队", Color.cyan, 28, 2f),
            new CreditsDividerData(15f, Color.gray, 1f),
            
            new CreditsTextData("角色原画：小美", Color.white, 24, 2f),
            new CreditsTextData("场景原画：小丽", Color.white, 24, 2f),
            new CreditsTextData("UI设计：小华", Color.white, 24, 2f),
            new CreditsTextData("特效设计：小强", Color.white, 24, 2f),
            
            new CreditsDividerData(25f, Color.gray, 1.5f),
            
            // 策划团队
            new CreditsTextData("策划团队", Color.cyan, 28, 2f),
            new CreditsDividerData(15f, Color.gray, 1f),
            
            new CreditsTextData("系统策划：小张", Color.white, 24, 2f),
            new CreditsTextData("数值策划：小王", Color.white, 24, 2f),
            new CreditsTextData("剧情策划：小李", Color.white, 24, 2f),
            
            new CreditsDividerData(30f, Color.gray, 1.5f),
            
            // 特别感谢
            new CreditsTextData("特别感谢", Color.yellow, 32, 3f),
            new CreditsDividerData(20f, Color.gray, 1f),
            
            new CreditsTextData("感谢所有支持我们的玩家", Color.white, 24, 3f),
            new CreditsTextData("感谢Unity引擎提供的技术支持", Color.white, 20, 2f),
            new CreditsTextData("感谢所有参与测试的朋友们", Color.white, 20, 2f),
            
            new CreditsDividerData(40f, Color.gray, 2f),
            
            // 结束语
            new CreditsTextData("游戏制作完成", Color.green, 28, 4f),
            new CreditsTextData("期待与您的下次相遇", Color.white, 24, 3f),
        };
    }
    
    /// <summary>
    /// 播放示例制作人员名单
    /// </summary>
    [ContextMenu("播放示例制作人员名单")]
    public void PlayExampleCredits()
    {
        if (creditsRoll != null)
        {
            creditsRoll.SetCreditsData(exampleCredits);
            creditsRoll.PlayCredits();
        }
    }
    
    /// <summary>
    /// 播放自定义制作人员名单
    /// </summary>
    public void PlayCustomCredits(List<CreditsRollData> customCredits)
    {
        if (creditsRoll != null)
        {
            creditsRoll.SetCreditsData(customCredits);
            creditsRoll.PlayCredits();
        }
    }
    
    /// <summary>
    /// 停止播放
    /// </summary>
    [ContextMenu("停止播放")]
    public void StopCredits()
    {
        if (creditsRoll != null)
        {
            creditsRoll.StopCredits();
        }
    }
    
    /// <summary>
    /// 创建简单的文本制作人员名单
    /// </summary>
    public static List<CreditsRollData> CreateSimpleCredits()
    {
        return new List<CreditsRollData>
        {
            new CreditsTitleData("制作人员", Color.yellow, 36, 3f),
            new CreditsDividerData(30f, Color.gray, 1f),
            new CreditsTextData("程序：张三", Color.white, 24, 2f),
            new CreditsTextData("美术：李四", Color.white, 24, 2f),
            new CreditsTextData("策划：王五", Color.white, 24, 2f),
            new CreditsDividerData(20f, Color.gray, 1f),
            new CreditsTextData("感谢游玩", Color.cyan, 28, 3f),
        };
    }
    
    /// <summary>
    /// 创建带头像的制作人员名单
    /// </summary>
    public static List<CreditsRollData> CreateAvatarCredits()
    {
        return new List<CreditsRollData>
        {
            new CreditsTitleData("核心团队", Color.yellow, 36, 3f),
            new CreditsDividerData(30f, Color.gray, 1f),
            new CreditsAvatarData("张三", "程序", "Avatars/Programmer", null, 4f),
            new CreditsAvatarData("李四", "美术", "Avatars/Artist", null, 4f),
            new CreditsAvatarData("王五", "策划", "Avatars/Designer", null, 4f),
            new CreditsDividerData(20f, Color.gray, 1f),
            new CreditsTextData("感谢支持", Color.cyan, 28, 3f),
        };
    }
}

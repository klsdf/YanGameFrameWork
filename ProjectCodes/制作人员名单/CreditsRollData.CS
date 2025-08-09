using UnityEngine;

/// <summary>
/// 制作人员名单数据基类
/// </summary>
public abstract class CreditsRollData
{
    public string id;
    public float displayTime = 2f; // 显示时间（秒）
    
    protected CreditsRollData(string id, float displayTime = 2f)
    {
        this.id = id;
        this.displayTime = displayTime;
    }
    
    /// <summary>
    /// 获取显示内容
    /// </summary>
    public abstract object GetDisplayContent();
}

/// <summary>
/// 纯文本类型
/// </summary>
public class CreditsTextData : CreditsRollData
{
    public string text;
    public Color textColor = Color.white;
    public int fontSize = 24;
    
    public CreditsTextData(string text, Color? textColor = null, int fontSize = 24, float displayTime = 2f) 
        : base(text, displayTime)
    {
        this.text = text;
        this.textColor = textColor ?? Color.white;
        this.fontSize = fontSize;
    }
    
    public override object GetDisplayContent() => text;
}

/// <summary>
/// 带头像的类型
/// </summary>
public class CreditsAvatarData : CreditsRollData
{
    public string name;
    public string role;
    public string avatarPath;
    public Sprite avatar;
    
    public CreditsAvatarData(string name, string role, string avatarPath = null, Sprite avatar = null, float displayTime = 3f) 
        : base($"{name}_{role}", displayTime)
    {
        this.name = name;
        this.role = role;
        this.avatarPath = avatarPath;
        this.avatar = avatar;
    }
    
    public override object GetDisplayContent() => new { name, role, avatar, avatarPath };
}

/// <summary>
/// 分隔线类型
/// </summary>
public class CreditsDividerData : CreditsRollData
{
    public float height = 20f;
    public Color lineColor = Color.gray;
    
    public CreditsDividerData(float height = 20f, Color? lineColor = null, float displayTime = 1f) 
        : base("divider", displayTime)
    {
        this.height = height;
        this.lineColor = lineColor ?? Color.gray;
    }
    
    public override object GetDisplayContent() => new { height, lineColor };
}

/// <summary>
/// 标题类型
/// </summary>
public class CreditsTitleData : CreditsRollData
{
    public string title;
    public Color titleColor = Color.yellow;
    public int fontSize = 32;
    
    public CreditsTitleData(string title, Color? titleColor = null, int fontSize = 32, float displayTime = 3f) 
        : base(title, displayTime)
    {
        this.title = title;
        this.titleColor = titleColor ?? Color.yellow;
        this.fontSize = fontSize;
    }
    
    public override object GetDisplayContent() => title;
}


using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 制作人员名单项目渲染器接口
/// </summary>
public interface ICreditsRenderer
{
    void Render(CreditsRollData data);
    void Clear();
}

/// <summary>
/// 制作人员名单项目基类
/// </summary>
public abstract class CreditsRollItem : MonoBehaviour, ICreditsRenderer
{
    protected CreditsRollData data;
    
    public virtual void SetContent(CreditsRollData data)
    {
        this.data = data;
        Render(data);
    }
    
    public abstract void Render(CreditsRollData data);
    
    public virtual void Clear()
    {
        data = null;
    }
}

/// <summary>
/// 文本渲染器
/// </summary>
public class TextCreditsRollItem : CreditsRollItem
{
    [Header("文本组件")]
    public TMP_Text text;
    
    public override void Render(CreditsRollData data)
    {
        if (data is CreditsTextData textData)
        {
            text.text = textData.text;
            text.color = textData.textColor;
            text.fontSize = textData.fontSize;
        }
        else if (data is CreditsTitleData titleData)
        {
            text.text = titleData.title;
            text.color = titleData.titleColor;
            text.fontSize = titleData.fontSize;
        }
    }
}

/// <summary>
/// 头像渲染器
/// </summary>
public class AvatarCreditsRollItem : CreditsRollItem
{
    [Header("头像组件")]
    public Image avatarImage;
    public TMP_Text nameText;
    public TMP_Text roleText;
    
    public override void Render(CreditsRollData data)
    {
        if (data is CreditsAvatarData avatarData)
        {
            nameText.text = avatarData.name;
            roleText.text = avatarData.role;
            
            if (avatarData.avatar != null)
            {
                avatarImage.sprite = avatarData.avatar;
            }
            else if (!string.IsNullOrEmpty(avatarData.avatarPath))
            {
                // 异步加载头像
                LoadAvatarAsync(avatarData.avatarPath);
            }
        }
    }
    
    private async void LoadAvatarAsync(string avatarPath)
    {
        var sprite = await YanGF.Resources.LoadFromAddressablesAsync<Sprite>(avatarPath);
        if (sprite != null)
        {
            avatarImage.sprite = sprite;
        }
    }
}

/// <summary>
/// 分隔线渲染器
/// </summary>
public class DividerCreditsRollItem : CreditsRollItem
{
    [Header("分隔线组件")]
    public Image lineImage;
    
    public override void Render(CreditsRollData data)
    {
        if (data is CreditsDividerData dividerData)
        {
            lineImage.color = dividerData.lineColor;
            var rectTransform = lineImage.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, dividerData.height);
        }
    }
}

/// <summary>
/// 制作人员名单渲染器工厂
/// </summary>
public static class CreditsRendererFactory
{
    /// <summary>
    /// 根据数据类型创建对应的渲染器
    /// </summary>
    public static ICreditsRenderer CreateRenderer(CreditsRollData data, GameObject prefab, Transform parent)
    {
        GameObject instance = Object.Instantiate(prefab, parent);
        
        if (data is CreditsTextData || data is CreditsTitleData)
        {
            return instance.GetComponent<TextCreditsRollItem>();
        }
        else if (data is CreditsAvatarData)
        {
            return instance.GetComponent<AvatarCreditsRollItem>();
        }
        else if (data is CreditsDividerData)
        {
            return instance.GetComponent<DividerCreditsRollItem>();
        }
        
        Debug.LogWarning($"未找到数据类型 {data.GetType().Name} 的渲染器");
        return null;
    }
}



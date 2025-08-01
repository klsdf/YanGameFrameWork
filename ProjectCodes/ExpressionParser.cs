using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
public interface IExpression
{
    string Interpret(Context context);  // 解释方法，返回计算结果
}


//上下文类：存储变量和指令逻辑
public class Context
{

    // private CardData _cardData;

    public Context( )
    {
        // _cardData = cardData;
    }



    // public string GetCardBaseInfo(string fieldName)
    // {
    //     object value = GetFieldValue(_cardData, fieldName);
    //     if (value != null)
    //     {
    //         return value.ToString();
    //     }
    //     else
    //     {
    //         Debug.LogError("没有找到值：" + fieldName);
    //         return "";
    //     }
    // }


    private object GetFieldValue(object obj, string fieldName)
    {
        if (obj == null)
        {
            Debug.LogError("对象为空，无法获取字段值。");
            return null;
        }

        // if (obj is not CardData)
        // {
        //     Debug.LogError("对象不是CardData，无法获取字段值。");
        //     return null;
        // }

        var type = obj.GetType();
        var fieldInfo = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (fieldInfo != null)
        {
            return fieldInfo.GetValue(obj);
        }

        var propertyInfo = type.GetProperty(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (propertyInfo != null)
        {
            return propertyInfo.GetValue(obj);
        }

        Debug.LogError($"字段或属性 '{fieldName}' 在对象 '{type.Name}' 中不存在。");
        return null;
    }


    public string GetLinkStr(string str)
    {
        return $"<link id=\"{str}\">{str}</link>";
    }
}




public class TextExpression : IExpression

{
    private string _text;

    public TextExpression(string text)
    {
        _text = text;
    }

    public string Interpret(Context context)
    {
        return _text;
    }
}



public class StyleExpression : IExpression
{
    private string _text;



    public StyleExpression(string text)
    {
        _text = text;
    }

    public string Interpret(Context context)
    {
        string color = "bfc";
        _text = context.GetLinkStr(_text);
        return $"<color=#{color}>{_text}</color>";
    }

}

public class NumberExpression : IExpression
{
    private string _text;



    public NumberExpression(string text)
    {
        _text = text;
    }

    public string Interpret(Context context)
    {
        string color = "bfc";
        return $"<color=#{color}>{_text}</color>";
    }

}

public class IconExpression : IExpression
{
    private string _text;



    public IconExpression(string text)
    {
        _text = text;
    }

    public string Interpret(Context context)
    {
        string color = "afc";
        return $"<color=#{color}>{_text}</color>";
    }
}


public class LinkExpression : IExpression
{
    private string _text;


    public LinkExpression(string text)
    {
        _text = text;
    }
    // private string ReplaceKeyWord(string keyWords)
    // {
    //     // 从CardTag枚举获取所有tag并替换为可点击链接
    //     string[] tags = System.Enum.GetNames(typeof(CardTag));

    //     foreach (string tag in tags)
    //     {
    //         keyWords = keyWords.Replace(tag, $"<link id=\"{tag}\">{tag}</link>");
    //     }

    //     // 保留原有的"生成"替换
    //     keyWords = keyWords.Replace("生成", "<link id=\"生成\">生成</link>");
    //     keyWords = keyWords.Replace("摧毁", "<link id=\"摧毁\">摧毁</link>");
    //     keyWords = keyWords.Replace("吃掉", "<link id=\"吃掉\">吃掉</link>");
    //     keyWords = keyWords.Replace("偷取", "<link id=\"偷取\">偷取</link>");


    //     return keyWords;
    // }

    public string Interpret(Context context)
    {

        return $"<link id=\"{_text}\">{_text}</link>";
    }
}


public class VariableExpression : IExpression
{

    private string _variableName;

    public VariableExpression(string variableName)
    {
        _variableName = variableName;
    }

    public string Interpret(Context context)
    {
        string color = "bbc";
        return $"<color=#{color}>{_variableName}</color>";
    }
}

public static class ExpressionParser
{
    private static readonly Regex variablePattern = new Regex(@"{{(.*?)}}");
    private static readonly Regex stylePattern = new Regex(@"\[(.*?)\]");
    private static readonly Regex namePattern = new Regex(@"@@(.*?)@@");
    private static readonly Regex iconPattern = new Regex(@"#(.*?)#");
    private static readonly Regex numberPattern = new Regex(@"\b\d+\b");
    public static List<IExpression> Parse(string expression)
    {
        List<IExpression> expressions = new List<IExpression>();
        int lastIndex = 0;

        var matches = new List<Match>();
        matches.AddRange(variablePattern.Matches(expression));
        matches.AddRange(stylePattern.Matches(expression));
        matches.AddRange(namePattern.Matches(expression));
        matches.AddRange(iconPattern.Matches(expression));
        matches.AddRange(numberPattern.Matches(expression));
        matches.Sort((a, b) => a.Index.CompareTo(b.Index));

        foreach (Match match in matches)
        {
            int startIndex = match.Index;
            int endIndex = match.Index + match.Length;

            // 变量或指令前的普通文本
            if (startIndex > lastIndex)
            {
                expressions.Add(new TextExpression(expression.Substring(lastIndex, startIndex - lastIndex)));
            }

            // 解析变量或指令表达式
            if (match.Value.StartsWith("{{"))
            {
                string variableName = match.Groups[1].Value;
                expressions.Add(new VariableExpression(variableName));
            }
            //解析样式
            else if (match.Value.StartsWith("[") && match.Value.EndsWith("]"))
            {
                string styleName = match.Groups[1].Value;
                expressions.Add(new StyleExpression(styleName));
            }
            else if (match.Value.StartsWith("@@") && match.Value.EndsWith("@@"))
            {
                string linkName = match.Groups[1].Value;
                Debug.Log("linkName: " + linkName);
                expressions.Add(new LinkExpression(linkName));
            }
            //解析图标
            else if (match.Value.StartsWith("#") && match.Value.EndsWith("#"))
            {
                string iconName = match.Groups[1].Value;
                expressions.Add(new IconExpression(iconName));
            }
            //解析数字
            else if (int.TryParse(match.Value, out _))
            {
                expressions.Add(new NumberExpression(match.Value));
            }

            lastIndex = endIndex;
        }

        // 添加剩余的普通文本
        if (lastIndex < expression.Length)
        {
            expressions.Add(new TextExpression(expression.Substring(lastIndex)));
        }

        return expressions;
    }


    public static string Interpret(string expression, Context context)
    {
        List<IExpression> expressionList = Parse(expression);
        StringBuilder result = new StringBuilder();

        foreach (var expressionItem in expressionList)
        {
            result.Append(expressionItem.Interpret(context));
        }
        return result.ToString();
    }

}

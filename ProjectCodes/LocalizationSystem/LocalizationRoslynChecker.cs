#if USE_ROSLYN
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;

#if UNITY_EDITOR
public static class LocalizationRoslynChecker
{
    // 需要本地化的类型
    private static readonly string[] _targetTypes = { "TMP_Text", "TextMeshProUGUI", "Text" };
    // 允许的本地化方法
    private static readonly string[] _localizationMethods = { "Translate" };

    public static void CheckLocalizationWithRoslyn(string folderPath = "Assets/YanGameFrameWork")
    {
        Debug.Log("开始检查未本地化字符串");
        var csFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);
        bool allPassed = true;
        foreach (var file in csFiles)
        {
            string content = File.ReadAllText(file);
            var tree = CSharpSyntaxTree.ParseText(content);
            var root = tree.GetRoot();

            // 收集所有字段/变量声明及其类型
            var semanticModel = GetSemanticModel(tree, file);
            if (semanticModel == null) continue;

            // 查找所有赋值表达式
            var assignments = root.DescendantNodes().OfType<AssignmentExpressionSyntax>()
                .Where(ae => ae.Left is MemberAccessExpressionSyntax maes && maes.Name.Identifier.Text == "text");

            foreach (var assign in assignments)
            {
                var maes = assign.Left as MemberAccessExpressionSyntax;
                var symbolInfo = semanticModel.GetSymbolInfo(maes.Expression);
                var symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();
                if (symbol == null) continue;
                var type = symbol switch
                {
                    ILocalSymbol local => local.Type,
                    IFieldSymbol field => field.Type,
                    IPropertySymbol prop => prop.Type,
                    IParameterSymbol param => param.Type,
                    _ => null
                };
                if (type == null) continue;
                if (!_targetTypes.Contains(type.Name)) continue;

                // 检查右侧是否为字符串字面量，且未被 Translate 包裹
                var right = assign.Right;
                bool needWarn = false;
                string strValue = null;
                if (right is LiteralExpressionSyntax les && les.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    strValue = les.Token.ValueText;
                    needWarn = true;
                }
                else if (right is InvocationExpressionSyntax ies)
                {
                    // 检查是否 Translate("xxx")
                    var expr = ies.Expression.ToString();
                    if (!_localizationMethods.Any(m => expr.Contains(m)))
                    {
                        needWarn = true;
                        strValue = right.ToString();
                    }
                }
                else
                {
                    // 其他情况（如插值字符串、拼接等）
                    if (right is InterpolatedStringExpressionSyntax || right is BinaryExpressionSyntax)
                    {
                        needWarn = true;
                        strValue = right.ToString();
                    }
                }

                if (needWarn && !string.IsNullOrWhiteSpace(strValue))
                {
                    allPassed = false;
                    var lineSpan = assign.GetLocation().GetLineSpan();
                    int lineNumber = lineSpan.StartLinePosition.Line + 1;
                    YanGF.Debug.LogWarning(nameof(LocalizationRoslynChecker),
                    $"[Roslyn本地化警告] 文件：<color=#00AFFF>{file}</color> 行号：{lineNumber}\n对{textType(type.Name)}.text赋值时未本地化：{strValue}");
                }
            }
        }
        if (allPassed)
        {
            Debug.Log("✅ 一切正常！");
        }
    }

    // 获取语义模型（简化版，适用于单文件分析）
    private static SemanticModel GetSemanticModel(SyntaxTree tree, string filePath)
    {
        try
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var unityEngine = MetadataReference.CreateFromFile(typeof(UnityEngine.Object).Assembly.Location);
            var compilation = CSharpCompilation.Create(
                "Analysis",
                new[] { tree },
                new[] { mscorlib, unityEngine },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );
            return compilation.GetSemanticModel(tree);
        }
        catch
        {
            return null;
        }
    }

    private static string textType(string typeName)
    {
        return _targetTypes.Contains(typeName) ? typeName : "未知类型";
    }
}
#endif
#endif
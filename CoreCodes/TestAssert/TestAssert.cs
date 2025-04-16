/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-16
 * Description: 通用测试断言工具类，支持值、集合、自定义对象的比较。
 *
 ****************************************************************************/

using System.Collections;
using System.Linq;
/// <summary>
/// 通用测试断言工具类，支持值、集合、自定义对象的比较。
/// </summary>
public static class TestAssert
{
    /// <summary>
    /// 断言两个对象相等（支持基本类型、集合、自定义对象）。
    /// </summary>
    public static void AreEqual<T>(T actual, T expected, string message = "")
    {
        if (IsEqual(actual, expected))
        {
            LogSuccess(message);
        }
        else
        {
            LogFailure(actual, expected, message);
        }
    }

    /// <summary>
    /// 断言两个对象不相等。
    /// </summary>
    public static void AreNotEqual<T>(T actual, T expected, string message = "")
    {
        if (!IsEqual(actual, expected))
        {
            LogSuccess(message);
        }
        else
        {
            LogFailure(actual, expected, message, notExpected: true);
        }
    }

    // --- 核心比较逻辑 ---
    private static bool IsEqual<T>(T a, T b)
    {
        // 如果两个对象是同一个对象，则返回true
        if (ReferenceEquals(a, b))
            return true;
        // 如果两个对象中有一个为null，则返回false
        if (a == null || b == null)
            return false;

        // 如果两个对象是集合类型，则比较集合内容
        if (IsCollection(a, b, out IEnumerable collectionA, out IEnumerable collectionB))
        {
            return CollectionsEqual(collectionA, collectionB);
        }

        // 如果两个对象是其它的对象则使用Equals方法比较
        return a.Equals(b);
    }

    // --- 辅助方法 ---
    private static bool IsCollection<T>(T obj, T expected, out IEnumerable collectionA, out IEnumerable collectionB)
    {
        collectionA = null;
        collectionB = null;

        if (obj is IEnumerable enumerable && !(obj is string)) // 排除 string
        {
            collectionA = enumerable;
            collectionB = expected as IEnumerable; // 使用参数 expected
            return true;
        }
        return false;
    }

    private static bool CollectionsEqual(IEnumerable a, IEnumerable b)
    {
        if (a == null || b == null)
            return a == b;

        var enumeratorA = a.GetEnumerator();
        var enumeratorB = b.GetEnumerator();

        while (true)
        {
            bool hasNextA = enumeratorA.MoveNext();
            bool hasNextB = enumeratorB.MoveNext();

            if (hasNextA != hasNextB)
                return false; // 长度不一致
            if (!hasNextA)
                return true; // 遍历结束且所有元素相等
            if (!Equals(enumeratorA.Current, enumeratorB.Current))
                return false;
        }
    }

    private static string FormatObject(object obj)
    {
        if (obj == null)
            return "null";

        IEnumerable collection;
        if (IsCollection(obj, obj, out collection, out _))
            return "[" + string.Join(", ", collection.Cast<object>()) + "]";

        return obj.ToString();
    }
    // --- 日志输出 ---
    private static void LogSuccess(string message)
    {
        if (!string.IsNullOrEmpty(message))
            YanGF.Debug.Log(nameof(TestAssert), $"测试通过: {message}");
        else
            YanGF.Debug.Log(nameof(TestAssert), "测试通过");
    }

    private static void LogFailure<T>(T actual, T expected, string message, bool notExpected = false)
    {
        string expectation = notExpected ? "预期不相等，但实际相等" : "预期相等，但实际不相等";
        string log = $"测试失败: {expectation}\n" +
                     $"预期: {FormatObject(expected)}\n" +
                     $"实际: {FormatObject(actual)}";

        if (!string.IsNullOrEmpty(message))
            log += $"\n上下文: {message}";

        YanGF.Debug.LogError(nameof(TestAssert), log);
    }
}
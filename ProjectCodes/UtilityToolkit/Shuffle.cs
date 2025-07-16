using System.Collections.Generic;
using UnityEngine;

public static class Shuffle
{

    /// <summary>
    /// Fisher-Yates洗牌算法，非破坏性算法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static List<T> Fisher_Yates_Shuffle_Algorithm<T>(List<T> cards)
    {
        // 复制一份用于打乱
        List<T> tempList = new List<T>(cards);


        //洗牌
        //从后往前遍历数组，每次随机选一个未处理的元素与当前位置交换。
        //这样可以保证每个排列出现的概率完全相等，是一种真正意义上的“等概率随机打乱”。
        for (int i = tempList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }

        return tempList;
    }
}

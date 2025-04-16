/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-16
 * Description: 游戏存档系统示例，以及单元测试用的类
 ****************************************************************************/
using UnityEngine;
using YanGameFrameWork.Editor;
using System.Collections.Generic;
using System;
using System.Linq;
namespace YanGameFrameWork.Example
{
    public class ExampleSaveSystem : MonoBehaviour
    {
        [Serializable]
        private class TestData
        {
            public int intValue;
            public string stringValue;
            public int[] intArray;
            public List<int> intList;

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                TestData other = (TestData)obj;

                // 比较 intValue 和 stringValue
                bool isEqual = intValue == other.intValue &&
                              stringValue == other.stringValue;

                // 比较 intArray（处理 null 情况）
                if (intArray == null && other.intArray == null)
                {
                    // 两者都为 null，视为相等
                }
                else if (intArray == null || other.intArray == null)
                {
                    isEqual = false;  // 一个为 null，另一个非 null，不相等
                }
                else
                {
                    isEqual &= intArray.SequenceEqual(other.intArray);  // 比较数组内容
                }

                // 比较 intList（处理 null 情况）
                if (intList == null && other.intList == null)
                {
                    // 两者都为 null，视为相等
                }
                else if (intList == null || other.intList == null)
                {
                    isEqual = false;  // 一个为 null，另一个非 null，不相等
                }
                else
                {
                    isEqual &= intList.SequenceEqual(other.intList);  // 比较列表内容
                }

                return isEqual;
            }

            public override int GetHashCode()
            {
                int hash = 17;
                hash = hash * 23 + intValue.GetHashCode();
                hash = hash * 23 + (stringValue?.GetHashCode() ?? 0);

                // 计算 intArray 的哈希（基于内容）
                if (intArray != null)
                {
                    foreach (int num in intArray)
                    {
                        hash = hash * 23 + num.GetHashCode();
                    }
                }
                else
                {
                    hash = hash * 23 + 0;  // null 视为 0
                }

                // 计算 intList 的哈希（基于内容）
                if (intList != null)
                {
                    foreach (int num in intList)
                    {
                        hash = hash * 23 + num.GetHashCode();
                    }
                }
                else
                {
                    hash = hash * 23 + 0;  // null 视为 0
                }

                return hash;
            }
        }


        [Button("保存数据int")]
        public void SaveDataInt()
        {
            int testInt = 123;
            YanGF.Save.SetSaveFileName("TestSave").Save("testInt", testInt);


            int result = YanGF.Save.Load<int>("testInt");

            TestAssert.AreEqual(result, testInt);
        }


        [Button("保存数据string")]
        public void SaveDataString()
        {
            string testString = "测试字符串";
            YanGF.Save.SetSaveFileName("TestSave").Save("testString", testString);

            string result = YanGF.Save.Load<string>("testString");

            TestAssert.AreEqual(result, testString);
        }



        [Button("保存数据int[]")]
        public void SaveDataArrayInt()
        {
            int[] testArrayInt = new int[] { 1, 2, 3 };
            YanGF.Save.SetSaveFileName("TestSave").Save("testArrayInt", testArrayInt);

            int[] result = YanGF.Save.Load<int[]>("testArrayInt");

            TestAssert.AreEqual(result, testArrayInt);
        }

        [Button("保存数据string[]")]
        public void SaveDataArrayString()
        {
            string[] testArrayString = new string[] { "1", "2", "3" };
            YanGF.Save.SetSaveFileName("TestSave").Save("testArrayString", testArrayString);

            string[] result = YanGF.Save.Load<string[]>("testArrayString");

            TestAssert.AreEqual(result, testArrayString);
        }

        [Button("保存数据List<int>")]
        public void SaveDataListInt()
        {
            List<int> testListInt = new List<int> { 1, 2, 3 };
            YanGF.Save.SetSaveFileName("TestSave").Save("testListInt", testListInt);

            List<int> result = YanGF.Save.Load<List<int>>("testListInt");

            TestAssert.AreEqual(result, testListInt);
        }

        [Button("保存数据TestData")]
        public void SaveDataTestData()
        {
            TestData testData = new TestData { intValue = 1, stringValue = "测试字符串", intArray = new int[] { 1, 2, 3 }, intList = new List<int> { 1, 2, 3 } };
            YanGF.Save.SetSaveFileName("TestSave").Save("testTestData", testData);

            TestData result = YanGF.Save.Load<TestData>("testTestData");

            TestAssert.AreEqual(result, testData);
        }





    }

}
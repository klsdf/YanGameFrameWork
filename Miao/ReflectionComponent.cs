using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;

namespace Miao
{
    public enum NameFlag
    {
        输入命名空间,
        不输入命名空间
    }



    static public class ReflectionComponent
    {
        static public void DeepCopyComponent(UnityEngine.Component 拷贝对象, UnityEngine.Component 赋值对象)
        {
            Type ComponentType1 = 拷贝对象.GetType();
            Type ComponentType2 = 赋值对象.GetType();

            foreach (PropertyInfo property in ComponentType2.GetProperties())//C1拷贝对象循环获取值。
            {
                if (property.CanWrite && property.Name != "name")
                {
                    //var value1 = ComponentType1.GetProperty(property.Name).GetValue(拷贝对象);
                    //var value2 = ComponentType2.GetProperty(property.Name).GetValue(赋值对象);
                    ComponentType2.GetProperty(property.Name).SetValue(赋值对象, property.GetValue(拷贝对象));//被拷贝对象C2进行赋值。

                }
            }
        }
        /// <summary>
        /// 按照需求，自动深赋值，会自动撇开不存在的string。用于供给不同对象
        /// </summary>
        /// <param name="拷贝对象"></param>
        /// <param name="赋值对象"></param>
        /// <param name="DataName"></param>
        static public void DeepCopyComponent(UnityEngine.Component 拷贝对象, UnityEngine.Component 赋值对象, params string[] DataName)
        {
            Type ComponentType1 = 拷贝对象.GetType();
            Type ComponentType2 = 赋值对象.GetType();

            var Intersect = GetPropertyIntersect(ComponentType1.GetProperties(), DataName).ToArray();
            for (int a = 0; a < Intersect.Length; a++)
            {
                //var value1 = ComponentType1.GetProperty(Intersect[a].Name).GetValue(拷贝对象);
                //var value2 = ComponentType2.GetProperty(Intersect[a].Name).GetValue(赋值对象);

                ComponentType2.GetProperty(Intersect[a].Name).SetValue(赋值对象, ComponentType1.GetProperty(Intersect[a].Name).GetValue(拷贝对象));
            }
        }
        static private IEnumerable<PropertyInfo> GetPropertyIntersect(PropertyInfo[] properties, string[] DataName)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                for (int j = 0; j < DataName.Length; j++)
                {
                    if (properties[i].Name == DataName[j])
                    {
                        yield return properties[i];
                    }
                }
            }

        }


    }
}

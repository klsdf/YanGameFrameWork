using UnityEngine;
using System;

namespace YanGameFrameWork.Editor
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string ButtonName { get; private set; }
        public float ButtonHeight { get; set; } = 30f;

        public ButtonAttribute()
        {
            ButtonName = null; // 如果为null则使用方法名
        }

        public ButtonAttribute(string buttonName)
        {
            ButtonName = buttonName;
        }
    }
}
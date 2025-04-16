// 这个文件必须放在Editor文件夹下！
using UnityEngine;
using UnityEditor;
using YanGameFrameWork.AchievementSystem;
using System.Linq;
using YanGameFrameWork.Editor;


[CustomEditor(typeof(AchievementSystem))]
public class AchievementSystemEditor : BaseEditor<AchievementSystem>
{
}


[CustomEditor(typeof(ExampleAchievementSystem))]
public class ExampleAchievementSystemEditor : BaseEditor<ExampleAchievementSystem>
{
}
// 这个文件必须放在Editor文件夹下！
using UnityEditor;
using YanGameFrameWork.SaveSystem;

[CustomEditor(typeof(SaveController))]
public class SaveControllerEditor : BaseEditor<SaveController>
{}

[CustomEditor(typeof(ExampleSaveSystem))]
public class ExampleSaveSystemEditor : BaseEditor<ExampleSaveSystem>
{}

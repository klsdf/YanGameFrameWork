// 这个文件必须放在Editor文件夹下！
using UnityEditor;
using YanGameFrameWork.SaveSystem;
using YanGameFrameWork.Example;
[CustomEditor(typeof(ReadConfigTableExample))]
public class ReadConfigEditor : BaseEditor<ReadConfigTableExample>
{}

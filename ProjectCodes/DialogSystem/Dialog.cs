/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-29
 * Description: 对话类的数据结构，包含对话内容和对话者
 *
 * 修改记录：
 ****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 具体的对话，包含对话内容和对话者
/// </summary>
[System.Serializable]
public class Dialog
{
    public string dialog;
    public string speaker;


    public override string ToString()
    {
        return $"{speaker}说: {dialog}";
    }
}



/// <summary>
/// 对话块，包含对话块的名称和对话列表
/// </summary>
[System.Serializable]
public class DialogBlock
{
    public string blockName;
    [SerializeField]
    private List<Dialog> _dialogs;
    private int _currentDialogIndex = 0;

    public DialogBlock(string blockName, List<Dialog> dialogs)
    {
        this.blockName = blockName;
        _dialogs = dialogs;
    }


    /// <summary>
    /// 获取下一个对话,如果对话结束则返回null
    /// </summary>
    /// <returns></returns>
    public Dialog GetNextDialog()
    {

        if (_currentDialogIndex >= _dialogs.Count)
        {
            YanGF.Debug.LogError(nameof(DialogController), "对话范围大于对话列表");
            return null;
        }

        if (_currentDialogIndex < 0)
        {
            YanGF.Debug.LogError(nameof(DialogController), "对话范围小于0");
            return null;
        }

        Dialog result = _dialogs[_currentDialogIndex];

        int nextIndex = _currentDialogIndex + 1;
        if (nextIndex >= _dialogs.Count)
        {
            _currentDialogIndex = _dialogs.Count - 1;
            YanGF.Debug.LogWarning(nameof(DialogController), "对话结束");
        }
        else
        {
            _currentDialogIndex = nextIndex;
        }

        return result;
    }




    /// <summary>
    /// 获取随机对话
    /// </summary>
    /// <returns></returns>
    public Dialog GetRandomDialog()
    {
        return _dialogs[Random.Range(0, _dialogs.Count)];
    }

    public bool IsDialogEnd()
    {
        return _currentDialogIndex >= _dialogs.Count;
    }


    public void Reset()
    {
        _currentDialogIndex = 0;
    }

    public void Reset(List<Dialog> dialogs)
    {
        _dialogs = dialogs;
        _currentDialogIndex = 0;
    }

}



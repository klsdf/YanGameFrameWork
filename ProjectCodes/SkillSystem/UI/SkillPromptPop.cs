/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-15
 * Description: 技能提示弹窗,需要子类重写
 *
 ****************************************************************************/

using YanGameFrameWork.UISystem;
using UnityEngine;

/// <summary>
/// 技能提示弹窗,也就是hover技能节点时显示的浮窗
/// 需要子类重写
/// </summary>
public abstract class SkillPromptPop : UIElementBase
{
    public abstract void ShowSkillPrompt(SkillNodeData skillNodeData, Vector3 position);
}

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
public abstract class SkillPromptPop : UIPanelBase
{
    /// <summary>
    /// 显示技能提示弹窗
    /// </summary>
    /// <param name="skillNodeData">技能节点数据</param>
    /// <param name="position">技能节点位置</param>
    public abstract void ShowSkillPrompt(SkillNodeData skillNodeData, Vector3 position);
}

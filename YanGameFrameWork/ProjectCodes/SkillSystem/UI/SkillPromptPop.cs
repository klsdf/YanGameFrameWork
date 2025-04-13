using YanGameFrameWork.UISystem;
using TMPro;

public class SkillPromptPop : UIElementBase
{
    public TMP_Text skillNameText;
    public TMP_Text skillDescriptionText;
    public void ShowSkillPrompt(SkillNodeData skillNodeData)
    {
        skillNameText.text = skillNodeData.Name;
        skillDescriptionText.text = skillNodeData.Description;
    }
}

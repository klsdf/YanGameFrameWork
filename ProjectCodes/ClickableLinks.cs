using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ClickableLinks : MonoBehaviour
{
    private TextMeshProUGUI m_TextMeshPro;
    private Canvas m_Canvas;
    private Camera m_Camera;

    public string[] keywords;
    private int currentHoveredLinkIndex = -1;
    void Awake()
    {
        m_TextMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        m_Canvas = gameObject.GetComponentInParent<Canvas>();
        // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
        if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            m_Camera = null;
        else
            m_Camera = m_Canvas.worldCamera;
    }

    void Update()
    {
        CheckMouseOverLink();
    }


    private void CheckMouseOverLink()
    {
        Vector3 mousePosition = Input.mousePosition;

        // 使用改进的链接检测方法
        var linkTaggedText = FindLinkAtMousePosition(mousePosition);

        if (linkTaggedText != -1 && linkTaggedText != currentHoveredLinkIndex)
        {
            currentHoveredLinkIndex = linkTaggedText;
            TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkTaggedText];
            string linkText = linkInfo.GetLinkText();
            Debug.Log("悬停到链接: " + linkText);
            EnterLink(linkText);

        }
        else if (linkTaggedText == -1 && currentHoveredLinkIndex != -1)
        {
            Debug.Log("离开链接");
            currentHoveredLinkIndex = -1;

            ExitLink();
        }
    }

    private int FindLinkAtMousePosition(Vector3 mousePosition)
    {
        // ScreenSpaceOverlay模式下使用null摄像机参数
        if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, mousePosition, null);
        }

        // 其他模式使用原始方法
        return TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, mousePosition, m_Canvas.worldCamera);
    }




    void EnterLink(string linkText)
    {
        print("EnterLink: " + linkText);
    }

    void ExitLink()
    {
        print("ExitLink");
    }




    public void OnPointerEnter(PointerEventData eventData)
    {
        // if (m_TextMeshPro.textInfo.linkCount > 0)
        // {

        //     //增加下划线
        //     m_TextMeshPro.fontStyle |= FontStyles.Underline;
        // }
        // print("OnPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //取消下划线
        // m_TextMeshPro.fontStyle &= ~FontStyles.Underline;

    }

}
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class CircleText2 : MonoBehaviour
{
    public TMP_Text targetText;
    public Vector2 center = Vector2.zero;
    public float radius = 100f;
    public float startAngle = 0f; // 可选：起始角度

    void Update()
    {
        if (targetText == null) return;
        ArrangeTextInCircle();
    }

    void ArrangeTextInCircle()
    {
        targetText.ForceMeshUpdate();
        var textInfo = targetText.textInfo;
        int charCount = textInfo.characterCount;
        if (charCount == 0) return;

        for (int i = 0; i < charCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            float angle = startAngle + (360f / charCount) * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector2 pos = center + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;

            // 计算旋转
            Quaternion rot = Quaternion.Euler(0, 0, angle + 90f);

            int vertexIndex = charInfo.vertexIndex;
            Vector3 charMid = (charInfo.bottomLeft + charInfo.topRight) / 2;

            int meshVertCount = targetText.textInfo.meshInfo[0].vertices.Length;
            if (vertexIndex >= 0 && vertexIndex + 3 < meshVertCount)
            {
                for (int j = 0; j < 4; j++)
                {
                    int idx = vertexIndex + j;
                    Vector3 orig = targetText.textInfo.meshInfo[0].vertices[idx];
                    Vector3 offset = orig - charMid;
                    offset = rot * offset;
                    targetText.textInfo.meshInfo[0].vertices[idx] = (Vector3)pos + offset;
                }
            }
        }
        targetText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
}

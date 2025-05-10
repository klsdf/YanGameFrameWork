/****************************************************************************
 * Author: 闫辰祥
 * Date: 2024-12-24
 * Description: 控制文本围绕圆形排列的类，最早是用于制作《我思故我在》的结局效果
 *
 ****************************************************************************/

using UnityEngine;
using TMPro;

namespace YanGameFrameWork.PracticalLibrary
{
    /// <summary>
    /// 控制文本围绕圆形排列的类
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class CircleText : MonoBehaviour
    {
        /// <summary>
        /// 文本组件
        /// </summary>
        private TMP_Text _textMeshPro;

        /// <summary>
        /// 圆的半径
        /// </summary>
        public float radius = 100f;

        /// <summary>
        /// 文本的旋转角度
        /// </summary>
        public float rotationAngle = 0f;

        private void Awake()
        {
            _textMeshPro = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            UpdateTextPosition();
        }

        /// <summary>
        /// 更新文本的位置，使其围绕圆形排列
        /// </summary>
        private void UpdateTextPosition()
        {
            // 获取文本的字符信息
            _textMeshPro.ForceMeshUpdate();
            var textInfo = _textMeshPro.textInfo;

            // 计算每个字符的位置
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                // 计算字符的角度
                float angle = (360f / textInfo.characterCount) * i + rotationAngle;
                float radian = angle * Mathf.Deg2Rad;

                // 计算字符的位置
                Vector3 offset = new Vector3(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius, 0);
                Vector3 charMidBaselinePos = (charInfo.bottomLeft + charInfo.topRight) / 2;
                Vector3 worldPos = transform.TransformPoint(charMidBaselinePos + offset);

                // 更新字符的位置
                int vertexIndex = charInfo.vertexIndex;
                for (int j = 0; j < 4; j++)
                {
                    _textMeshPro.textInfo.meshInfo[0].vertices[vertexIndex + j] += offset;
                }
            }

            // 更新网格
            _textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }

}
namespace YanGameFrameWork.UtilityToolkit
{
    using UnityEngine;
    public static class YanMathUtil
    {



        /// <summary>
        /// 给传入的3维坐标生成一个随机的2维偏移量
        /// </summary>
        /// <param name="position">初始坐标</param>
        /// <returns>初始坐标随机偏移之后的坐标</returns>
        public static Vector3 MakeRandomOffset(Vector3 position)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0f);
            return position + randomOffset;
        }

        /// <summary>
        /// 根据当前值，标准值，缩放系数，计算缩放值，最小的缩放值为1,最大的缩放值为10
        /// </summary>
        /// <param name="currentValue"> 当前值 </param>
        /// <param name="standardValue"> 标准值，决定了x的增长速度 </param>
        /// <param name="scaleDivisor"> 缩放系数，计算出结果之后再缩放 </param>
        /// <returns> 缩放值 </returns>
        public static float GetScaleByValue(float currentValue, float standardValue, float scaleDivisor)
        {
            const float MinScale = 1;
            const float MaxScale = 10;

            // 加1以避免log(0)
            float logValue = Mathf.Log(1 + Mathf.Abs(currentValue / standardValue));
            if (logValue <= MinScale)
            {
                return MinScale;
            }
            else if (logValue > MinScale && logValue < MaxScale)
            {
                return (logValue - MinScale) / scaleDivisor + MinScale;
            }
            else
            {
                return MaxScale;
            }

        }

    }






}
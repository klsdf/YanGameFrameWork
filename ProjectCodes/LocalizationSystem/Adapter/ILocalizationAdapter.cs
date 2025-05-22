/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-05-21 14:25
 * Description: 本地化适配器接口
 ****************************************************************************/

namespace YanGameFrameWork.LocalizationSystem
{
    public interface ILocalizationAdapter
    {

        string Translate(string key, MetaData metaData, string chineseText = null);
        void SwitchLanguage(LanguageType language);
        LanguageType GetCurrentLanguage();
        void OnDestroy();

    }
}
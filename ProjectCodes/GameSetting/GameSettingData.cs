using System;

namespace YanGameFrameWork.GameSetting
{
    [Serializable]
    public class GameSettingData
    {
        public float masterVolume;
        public float musicVolume;
        public float effectsVolume;
        public int languageIndex;
        public int resolutionIndex;
        public bool isFullScreen;
        public bool isFrameLocked;


        public GameSettingData()
        {
            masterVolume = 0.5f;
            musicVolume = 0.5f;
            effectsVolume = 0.5f;
            languageIndex = 0;
            resolutionIndex = 0;
            isFullScreen = true;
            isFrameLocked = true;
        }


    }

}


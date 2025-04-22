/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-20
 * Description: 音频系统，负责播放音频。拥有缓存机制。
 *

 * 修改记录:
* 2025-04-22 闫辰祥 增加了音频系统的混音器，并增加了总音量，音乐音量，音效音量的调节
 ****************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // 引入Audio命名空间
using YanGameFrameWork.CoreCodes;

namespace YanGameFrameWork.AudioSystem
{
    public class AudioController : Singleton<AudioController>
    {
        public AudioMixer audioMixer; // 引用AudioMixer
        public AudioMixerGroup masterGroup; // 总音量组
        public AudioMixerGroup bgmGroup; // 背景音乐组
        public AudioMixerGroup seGroup; // 音效组

        private List<AudioSource> _audioSourcePool = new List<AudioSource>();
        private int _initialPoolSize = 5; // 初始音源池大小

        protected override void Awake()
        {
            base.Awake();
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < _initialPoolSize; i++)
            {
                CreateNewAudioSource();
            }
        }

        private AudioSource CreateNewAudioSource()
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            _audioSourcePool.Add(newSource);
            return newSource;
        }

        private AudioSource GetAudioSource()
        {
            foreach (var source in _audioSourcePool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }
            // 如果没有可用的音源，创建一个新的
            return CreateNewAudioSource();
        }

        /// <summary>
        /// 停止所有音频
        /// </summary>
        // [Button("停止所有音频")]
        public void StopAllSounds()
        {
            foreach (var source in _audioSourcePool)
            {
                source.Stop();
            }
        }

        /// <summary>
        /// 播放循环音频
        /// </summary>
        /// <param name="clip">音频剪辑</param>
        /// <returns>用于播放音频的AudioSource</returns>
        /// <exception cref="System.ArgumentNullException">当clip为空时抛出</exception>
        public AudioSource PlayLoop(AudioClip clip)
        {
            if (clip == null)
            {
                throw new System.ArgumentNullException(nameof(clip), "音频剪辑不能为空");
            }

            AudioSource source = GetAudioSource();
            source.clip = clip;
            source.loop = true;
            source.outputAudioMixerGroup = bgmGroup;
            source.Play();
            return source;
        }

        /// <summary>
        /// 播放一次性音频
        /// </summary>
        /// <param name="clip">音频剪辑</param>
        /// <returns>用于播放音频的AudioSource</returns>
        /// <exception cref="System.ArgumentNullException">当clip为空时抛出</exception>
        public AudioSource PlayOnce(AudioClip clip)
        {
            if (clip == null)
            {
                throw new System.ArgumentNullException(nameof(clip), "音频剪辑不能为空");
            }

            AudioSource source = GetAudioSource();
            source.clip = clip;
            source.loop = false;
            source.outputAudioMixerGroup = seGroup;
            source.Play();
            return source;
        }

        /// <summary>
        /// 设置主音量
        /// </summary>
        /// <param name="volume">音量值（通常在-80到20之间）</param>
        public void SetMasterVolume(float volume)
        {
            if(volume < -80f || volume > 20f)
            {
                YanGF.Debug.LogWarning(nameof(AudioController), "音量超过了正常大小，但是已经调整到了正常值。");
            }
            volume = Mathf.Clamp(volume, -80f, 20f);
            audioMixer.SetFloat("MasterVolume", volume);
        }

        /// <summary>
        /// 设置音乐音量
        /// </summary>
        /// <param name="volume">音量值（通常在-80到20之间）</param>
        public void SetMusicVolume(float volume)
        {
            if(volume < -80f || volume > 20f)
            {
                YanGF.Debug.LogWarning(nameof(AudioController), "音量超过了正常大小，但是已经调整到了正常值。");
            }
            volume = Mathf.Clamp(volume, -80f, 20f);
            audioMixer.SetFloat("MusicVolume", volume);
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        /// <param name="volume">音量值（通常在-80到20之间）</param>
        public void SetEffectsVolume(float volume)
        {
            if(volume < -80f || volume > 20f)
            {
                YanGF.Debug.LogWarning(nameof(AudioController), "音量超过了正常大小，但是已经调整到了正常值。");
            }
            volume = Mathf.Clamp(volume, -80f, 20f);
            audioMixer.SetFloat("EffectsVolume", volume);
        }
    }
}
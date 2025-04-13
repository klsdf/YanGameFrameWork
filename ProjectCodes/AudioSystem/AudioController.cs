/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-20
 * Description: 音频系统，负责播放音频。拥有缓存机制。
 *
 ****************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using YanGameFrameWork.CoreCodes;


namespace YanGameFrameWork.AudioSystem
{
    public class AudioController : Singleton<AudioController>
    {

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
        /// <param name="clip">音频</param>
        public AudioSource PlayLoop(AudioClip clip)
        {
            AudioSource source = GetAudioSource();
            source.clip = clip;
            source.loop = true;
            source.Play();
            return source;
        }

        /// <summary>
        /// 播放一次性音频
        /// </summary>
        /// <param name="clip">音频</param>
        public AudioSource PlayOnce(AudioClip clip)
        {
            AudioSource source = GetAudioSource();
            source.clip = clip;
            source.loop = false;
            source.Play();
            return source;
        }

    }

}
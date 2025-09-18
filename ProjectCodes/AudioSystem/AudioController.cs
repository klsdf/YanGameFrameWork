/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-03-20
 * Description: 音频系统，负责播放音频。拥有缓存机制。
 *

 * 修改记录:
* 2025-04-22 闫辰祥 增加了音频系统的混音器，并增加了总音量，音乐音量，音效音量的调节
* 2025-05-29 闫辰祥 增加了多个音源池，并删除了playonce和playloop，使用了更加表层的播放方法，可以直接playbgm，playbgs，playse
 ****************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // 引入Audio命名空间
using YanGameFrameWork.Singleton;
using System.Collections;
using Sirenix.OdinInspector;

namespace YanGameFrameWork.AudioSystem
{

    public enum AudioType
    {
        BGM,
        BGS,
        SE
    }



    public class AudioController : Singleton<AudioController>
    {
        public AudioMixer audioMixer; // 引用AudioMixer
        public AudioMixerGroup masterGroup; // 总音量组
        public AudioMixerGroup bgmGroup; // 背景音乐组
        public AudioMixerGroup bgsGroup; // 背景音效组
        public AudioMixerGroup seGroup; // 音效组

        // Mixer 参数名常量（需与 AudioMixer Exposed 参数一致）
        private const string MasterAttenuationVolume = "MasterAttenuationVolume";
        private const string BGMAttenuationVolume = "BGMAttenuationVolume";
        private const string SEAttenuationVolume = "SEAttenuationVolume";
        private const string BGSAttenuationVolume = "BGSAttenuationVolume";

        // BGM 组效果器参数名（Lowpass / ParamEQ）
        private const string BGMLowpassCutoff = "BGMLowpassCutoff"; // Hz
        private const string BGMLowpassResonance = "BGMLowpassResonance"; // Q
        private const string BGMParamEQCenterFreq = "BGMParamEQCenterFreq"; // Hz
        private const string BGMParamEQOctaveRange = "BGMParamEQOctaveRange"; // octave
        private const string BGMParamEQGain = "BGMParamEQGain"; // dB

        // 为每种音频类型创建单独的音源池
        private List<AudioSource> _bgmSourcePool = new List<AudioSource>(); // 背景音乐音源池
        private List<AudioSource> _bgsSourcePool = new List<AudioSource>(); // 背景音效音源池
        private List<AudioSource> _seSourcePool = new List<AudioSource>();  // 音效音源池



        private GameObject _bgmObject;
        private GameObject _bgsObject;
        private GameObject _seObject;



        private int _initialPoolSize = 5; // 初始音源池大小

        protected void Awake()
        {
            CreateAudioSourcePool(AudioType.BGM, _bgmSourcePool);
            CreateAudioSourcePool(AudioType.BGS, _bgsSourcePool);
            CreateAudioSourcePool(AudioType.SE, _seSourcePool);
        }

        /// <summary>
        /// 创建音源池并挂载到子物体
        /// </summary>
        /// <param name="name">子物体名称</param>
        /// <param name="pool">音源池</param>
        /// <param name="group">音频混音组</param>
        private void CreateAudioSourcePool(AudioType type, List<AudioSource> pool)
        {
            GameObject child = new GameObject(type.ToString());
            switch (type)
            {
                case AudioType.BGM:
                    _bgmObject = child;
                    break;
                case AudioType.BGS:
                    _bgsObject = child;
                    break;
                case AudioType.SE:
                    _seObject = child;
                    break;
            }
            child.transform.SetParent(this.transform);
            InitializePool(pool, child);
        }

        /// <summary>
        /// 初始化音源池
        /// </summary>
        /// <param name="pool">音源池</param>
        /// <param name="parent">父物体</param>
        private void InitializePool(List<AudioSource> pool, GameObject parent)
        {
            for (int i = 0; i < _initialPoolSize; i++)
            {
                CreateNewAudioSource(pool, parent);
            }
        }

        private AudioSource CreateNewAudioSource(List<AudioSource> pool, GameObject parent)
        {
            AudioSource newSource = parent.AddComponent<AudioSource>();
            pool.Add(newSource);
            return newSource;
        }


        /// <summary>
        /// 获取一个可用的音源
        /// </summary>
        /// <param name="pool">音源池</param>
        /// <param name="parent">父物体</param>
        /// <returns>可用的音源</returns>
        private AudioSource GetAudioSource(List<AudioSource> pool, GameObject parent)
        {
            foreach (var source in pool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }
            // 如果没有可用的音源，创建一个新的
            return CreateNewAudioSource(pool, parent);
        }



        #region 停止音频

        /// <summary>
        /// 停止所有音频
        /// </summary>
        /// <param name="fadeOut">是否启用淡出效果</param>
        public void StopAllSounds(bool fadeOut = false)
        {
            StopAudioSources(_bgmSourcePool, fadeOut);
            StopAudioSources(_bgsSourcePool, fadeOut);
            StopAudioSources(_seSourcePool, fadeOut);
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        /// <param name="fadeOut">是否启用淡出效果</param>
        public void StopMusic(bool fadeOut = false)
        {
            StopAudioSources(_bgmSourcePool, fadeOut);
        }

        /// <summary>
        /// 停止背景音效
        /// </summary>
        /// <param name="fadeOut">是否启用淡出效果</param>
        public void StopBGS(bool fadeOut = false)
        {
            StopAudioSources(_bgsSourcePool, fadeOut);
        }

        /// <summary>
        /// 停止音效
        /// </summary>
        /// <param name="fadeOut">是否启用淡出效果</param>
        public void StopSE(bool fadeOut = false)
        {
            StopAudioSources(_seSourcePool, fadeOut);
        }

        /// <summary>
        /// 停止音源池中的音频
        /// </summary>
        /// <param name="pool">音源池</param>
        /// <param name="fadeOut">是否启用淡出效果</param>
        private void StopAudioSources(List<AudioSource> pool, bool fadeOut)
        {
            foreach (var source in pool)
            {
                if (fadeOut)
                {
                    StartCoroutine(FadeOutAndStop(source));
                }
                else
                {
                    source.Stop();
                }
            }
        }

        /// <summary>
        /// 淡出并停止音频
        /// </summary>
        /// <param name="source">音源</param>
        /// <returns>协程</returns>
        private IEnumerator FadeOutAndStop(AudioSource source)
        {
            float startVolume = source.volume;
            float duration = 1.0f; // 淡出持续时间

            while (source.volume > 0)
            {
                source.volume -= startVolume * Time.deltaTime / duration;
                yield return null;
            }

            source.Stop();
            source.volume = startVolume; // 恢复音量
        }

        #endregion


        #region 音频播放


        public void PlayBGM(AudioClip clip, float volume = 1f)
        {
            Play(
                clip: clip,
                volume: volume,
                group: bgmGroup,
                pool: _bgmSourcePool,
                parent: _bgmObject,
                isLoop: true);
        }
        /// <summary>
        /// 协程：依次播放背景音乐片段，每个片段播放完毕后暂停5秒。
        /// </summary>
        /// <returns>协程迭代器。</returns>
        private IEnumerator PlayBGMClipsSequentially(AudioClip[] clips, float waitTime)
        {
            while (true) // 无限循环
            {
                foreach (var clip in clips)
                {
                    Play(
                    clip: clip,
                     volume: 1f,
                     group: bgmGroup,
                     pool: _bgmSourcePool,
                     parent: _bgmObject,
                     isLoop: false);
                    yield return new WaitForSeconds(clip.length); // 等待当前片段播放完毕
                    yield return new WaitForSeconds(waitTime); // 暂停几秒
                }
            }
        }

        /// <summary>
        /// 开始播放背景音乐序列。
        /// </summary>
        public void StartBGMSequence(AudioClip[] clips, float waitTime = 5f)
        {
            StartCoroutine(PlayBGMClipsSequentially(clips, waitTime));
        }




        public void PlayBGS(AudioClip clip, float volume = 1f)
        {
            Play(
                clip: clip,
                volume: volume,
                group: bgsGroup,
                pool: _bgsSourcePool,
                parent: _bgsObject,
                isLoop: true);
        }

        public void PlaySE(AudioClip clip, float volume = 1f)
        {
            Play(
                clip: clip,
                volume: volume,
                group: seGroup,
                pool: _seSourcePool,
                parent: _seObject,
                isLoop: false);
        }


        /// <summary>
        /// 随机播放音效
        /// </summary>
        /// <param name="clips">音效片段</param>
        /// <param name="volume">音量</param>
        public void PlayRandomSE(AudioClip[] clips, float volume = 1f)
        {
            PlaySE(clips[Random.Range(0, clips.Length)], volume);
        }







        /// <summary>
        /// 播放循环音频
        /// </summary>
        /// <param name="clip">音频剪辑</param>
        /// <returns>用于播放音频的AudioSource</returns>
        /// <exception cref="System.ArgumentNullException">当clip为空时抛出</exception>
        private AudioSource Play(AudioClip clip, float volume, AudioMixerGroup group, List<AudioSource> pool, GameObject parent, bool isLoop)
        {
            if (clip == null)
            {
                throw new System.ArgumentNullException(nameof(clip), "音频剪辑不能为空");
            }

            AudioSource source = GetAudioSource(pool, parent);
            source.clip = clip;
            source.loop = isLoop;
            source.outputAudioMixerGroup = group;
            source.volume = volume;
            source.Play();
            return source;
        }

        #endregion

        #region 音量调节

        /// <summary>
        /// 设置主音量
        /// </summary>
        /// <param name="volume">音量值（通常在-80到20之间）</param>
        public void SetMasterVolume(float volume)
        {
            if (volume < -80f || volume > 20f)
            {
                YanGF.Debug.LogWarning(nameof(AudioController), "音量超过了正常大小，但是已经调整到了正常值。");
            }
            volume = Mathf.Clamp(volume, -80f, 20f);
            audioMixer.SetFloat(MasterAttenuationVolume, volume);
        }

        /// <summary>
        /// 设置音乐音量
        /// </summary>
        /// <param name="volume">音量值（通常在-80到20之间）</param>
        public void SetBGMVolume(float volume)
        {
            if (volume < -80f || volume > 20f)
            {
                YanGF.Debug.LogWarning(nameof(AudioController), "音量超过了正常大小，但是已经调整到了正常值。");
            }
            volume = Mathf.Clamp(volume, -80f, 20f);
            audioMixer.SetFloat(BGMAttenuationVolume, volume);
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        /// <param name="volume">音量值（通常在-80到20之间）</param>
        public void SetSEVolume(float volume)
        {
            if (volume < -80f || volume > 20f)
            {
                YanGF.Debug.LogWarning(nameof(AudioController), "音量超过了正常大小，但是已经调整到了正常值。");
            }
            volume = Mathf.Clamp(volume, -80f, 20f);
            audioMixer.SetFloat(SEAttenuationVolume, volume);
        }

        public void SetBGSVolume(float volume)
        {
            if (volume < -80f || volume > 20f)
            {
                YanGF.Debug.LogWarning(nameof(AudioController), "音量超过了正常大小，但是已经调整到了正常值。");
            }
            volume = Mathf.Clamp(volume, -80f, 20f);
            audioMixer.SetFloat(BGSAttenuationVolume, volume);
        }

        #endregion




        #region 效果器

        /// <summary>
        /// 启用或关闭 BGM Lowpass 滤波器。
        /// 需要将 Lowpass 的 Cutoff/Resonance 在 AudioMixer 中 Expose 对应参数名。
        /// 当关闭时，将 Cutoff 调高至全频，保持与打开前可逆。
        /// </summary>
        /// <param name="enabled">true=打开，false=关闭</param>
        /// <param name="cutoffHz">开启时的截止频率（Hz），例如 3000</param>
        /// <param name="resonance">开启时的共振(Q)值，例如 1.1</param>
        public void SetBGMLowpass(float cutoffHz = 3000f, float resonance = 1.1f)
        {
            if (audioMixer == null)
                return;

            audioMixer.SetFloat(BGMLowpassCutoff, Mathf.Clamp(cutoffHz, 20f, 22000f));
            audioMixer.SetFloat(BGMLowpassResonance, Mathf.Clamp(resonance, 0.1f, 10f));

        }

        /// <summary>
        /// 设置 BGM ParamEQ 峰值均衡参数。
        /// 需在 AudioMixer 中 Expose CenterFreq/OctaveRange/Gain。
        /// 若想“关闭”该效果，调用 <see cref="DisableBGMParamEQ"/> 将增益设为 0dB 即可。
        /// </summary>
        /// <param name="centerHz">中心频率（Hz）</param>
        /// <param name="octaveRange">带宽（octave）</param>
        /// <param name="gainDb">增益（dB），0 表示无增益，等效关闭</param>
        public void SetBGMParamEQ(float centerHz = 325f, float octaveRange = 1.18f, float gainDb = 1.75f)
        {
            if (audioMixer == null)
                return;
            audioMixer.SetFloat(BGMParamEQCenterFreq, Mathf.Clamp(centerHz, 20f, 20000f));
            audioMixer.SetFloat(BGMParamEQOctaveRange, Mathf.Max(0.01f, octaveRange));
            audioMixer.SetFloat(BGMParamEQGain, Mathf.Clamp(gainDb, -24f, 24f));
        }

        /// <summary>
        /// 关闭（听感上移除）BGM ParamEQ：将增益设为 0dB。
        /// </summary>
        public void DisableBGMParamEQ()
        {
            if (audioMixer == null)
                return;
            audioMixer.SetFloat(BGMParamEQGain, 1f);
        }

        /// <summary>
        /// 关闭（听感上移除）BGM Lowpass：将截止频率调至全频。
        /// </summary>
        public void DisableBGMLowpass()
        {
            if (audioMixer == null)
                return;
            audioMixer.SetFloat(BGMLowpassCutoff, 22000f);
        }

        #endregion


        #region 预制好的效果器

        [Button("设置水下效果")]

        public void SetUnderWaterEffect()
        {
            SetBGMParamEQ(
                   centerHz: 325f,
                   octaveRange: 1.18f,
                   gainDb: 1.75f);
            SetBGMLowpass(
                   cutoffHz: 1500f,
                   resonance: 1.18f);
        }

        [Button("关闭所有效果")]
        public void CloseAllEffect()
        {
            // 关闭 BGM 低通与参数均衡效果
            DisableBGMLowpass();
            DisableBGMParamEQ();
        }


        #endregion

    }
}
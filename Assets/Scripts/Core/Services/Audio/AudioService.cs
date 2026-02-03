using System.Collections.Generic;
using UnityEngine;

namespace Services.AudioService
{
    [System.Serializable]
    public class BGMSoundData
    {
        public enum BGM
        {
            Title,
            Stage
        }

        public BGM Bgm;
        public AudioClip AudioClip;
        [Range(0, 1)] public float Volume = 1;

    }

    [System.Serializable]
    public class SESoundData
    {
        public enum SE
        {
            Button,
            Puzzle,
            Item,
            Movement,
            Wrong,
        }

        public SE Se;
        public AudioClip AudioClip;
        [Range(0, 1)] public float Volume = 1;
    }

    /// <summary>
    /// Audio service for managing game audio
    /// </summary>
    public sealed class AudioService : MonoBehaviour, IAudioService
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _seSource;

        [Header("Audio Clip Libraries")]

        [SerializeField] List<BGMSoundData> bgmSoundDatas;
        [SerializeField] List<SESoundData> seSoundDatas;

        [Range(0, 1)] public float BgmMasterVolume = 1;
        [Range(0, 1)] public float SeMasterVolume = 1;

        public float BGMVolume => BgmMasterVolume;
        public float SEVolume => SeMasterVolume;

        public void PlayBGM(BGMSoundData.BGM bgm)
        {
            BGMSoundData data = bgmSoundDatas.Find(data => data.Bgm == bgm);
            _bgmSource.clip = data.AudioClip;
            _bgmSource.volume = data.Volume * BgmMasterVolume;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }

        public void StopBGM()
        {
            _bgmSource.loop = false;
            _bgmSource.Stop();
        }

        public void PlaySE(SESoundData.SE se)
        {
            SESoundData data = seSoundDatas.Find(data => data.Se == se);
            _seSource.volume = data.Volume * SeMasterVolume;
            _seSource.PlayOneShot(data.AudioClip);
        }

        public void StopSE()
        {
            _seSource.Stop();
        }

        public void SetBgmVolume(float volume01)
        {
            BgmMasterVolume = Mathf.Clamp01(volume01);
            _bgmSource.volume = BgmMasterVolume;
        }

        public void SetSeVolume(float volume01)
        {
            SeMasterVolume = Mathf.Clamp01(volume01);
            _seSource.volume = SeMasterVolume;
        }
    }
}

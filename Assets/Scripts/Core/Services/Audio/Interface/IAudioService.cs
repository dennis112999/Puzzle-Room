namespace Services.AudioService
{
    public interface IAudioService
    {
        float BGMVolume {get;}
        float SEVolume {get;}

        // BGM
        void PlayBGM(BGMSoundData.BGM bgm);
        void StopBGM();

        // SE
        void PlaySE(SESoundData.SE se);
        void StopSE();

        // Volume
        void SetBgmVolume(float volume);
        void SetSeVolume(float volume);
    }
}

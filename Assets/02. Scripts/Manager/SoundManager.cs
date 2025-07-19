using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Sound Data")]
    [SerializeField] private SoundDataSO _bgmData;
    [SerializeField] private SoundDataSO _sfxData;

    private AudioSource _bgmPlayer;
    private float _globalBgmVolume = 1.0f;
    private float _globalSfxVolume = 1.0f;

    private void Start()
    {
        _bgmPlayer = gameObject.AddComponent<AudioSource>();
        _bgmPlayer.loop = true;
    }

    public void PlayBGM(string name)
    {
        SoundClip soundClip = _bgmData.GetSoundClip(name);
        if (soundClip != null && _bgmPlayer.clip != soundClip.clip)
        {
            _bgmPlayer.clip = soundClip.clip;
            // 전체 BGM 볼륨과 클립 개별 볼륨을 곱해서 최종 볼륨 설정
            _bgmPlayer.volume = _globalBgmVolume * soundClip.volume;
            _bgmPlayer.Play();
        }
    }

    public void PlaySFX(string name)
    {
        SoundClip soundClip = _sfxData.GetSoundClip(name);
        if (soundClip != null)
        {
            GameObject sfxObject = new GameObject("SFXPlayer");
            AudioSource sfxPlayer = sfxObject.AddComponent<AudioSource>();

            // 전체 SFX 볼륨과 클립 개별 볼륨을 곱한 값을 PlayOneShot의 볼륨 스케일로 사용
            sfxPlayer.PlayOneShot(soundClip.clip, _globalSfxVolume * soundClip.volume);

            Destroy(sfxObject, soundClip.clip.length);
        }
    }

    public void SetBGMVolume(float volume)
    {
        _globalBgmVolume = Mathf.Clamp01(volume);
        // 현재 재생 중인 BGM의 볼륨도 즉시 업데이트
        if (_bgmPlayer.isPlaying && _bgmPlayer.clip != null)
        {
            SoundClip currentClip = _bgmData.GetSoundClip(_bgmPlayer.clip.name);
            if (currentClip != null)
            {
                _bgmPlayer.volume = _globalBgmVolume * currentClip.volume;
            }
        }
    }

    public void SetSFXVolume(float volume)
    {
        _globalSfxVolume = Mathf.Clamp01(volume);
    }
}
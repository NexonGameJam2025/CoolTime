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
            // ��ü BGM ������ Ŭ�� ���� ������ ���ؼ� ���� ���� ����
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

            // ��ü SFX ������ Ŭ�� ���� ������ ���� ���� PlayOneShot�� ���� �����Ϸ� ���
            sfxPlayer.PlayOneShot(soundClip.clip, _globalSfxVolume * soundClip.volume);

            Destroy(sfxObject, soundClip.clip.length);
        }
    }

    public void SetBGMVolume(float volume)
    {
        _globalBgmVolume = Mathf.Clamp01(volume);
        // ���� ��� ���� BGM�� ������ ��� ������Ʈ
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
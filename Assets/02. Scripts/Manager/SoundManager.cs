using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sound Data")]
    [SerializeField] private SoundDataSO _bgmData;
    [SerializeField] private SoundDataSO _sfxData;

    private AudioSource _bgmPlayer;
    private float _globalBgmVolume = 1.0f;
    private float _globalSfxVolume = 1.0f;
    private Coroutine _co_bgmSequence;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _bgmPlayer = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(string name, bool loop = true)
    {
        if (_co_bgmSequence != null)
        {
            StopCoroutine(_co_bgmSequence);
        }

        SoundClip soundClip = _bgmData.GetSoundClip(name);
        if (soundClip != null)
        {
            _bgmPlayer.clip = soundClip.clip;
            _bgmPlayer.volume = _globalBgmVolume * soundClip.volume;
            _bgmPlayer.loop = loop;
            _bgmPlayer.Play();
        }
    }

    public void PlayBGMWithIntro(string introName, string loopName, float introDuration = -1f)
    {
        if (_co_bgmSequence != null)
        {
            StopCoroutine(_co_bgmSequence);
        }
        _co_bgmSequence = StartCoroutine(CO_PlayBGMWithIntro(introName, loopName, introDuration));
    }

    private IEnumerator CO_PlayBGMWithIntro(string introName, string loopName, float introDuration)
    {
        SoundClip introClip = _bgmData.GetSoundClip(introName);
        if (introClip != null)
        {
            _bgmPlayer.clip = introClip.clip;
            _bgmPlayer.volume = _globalBgmVolume * introClip.volume;
            _bgmPlayer.loop = false;
            _bgmPlayer.Play();

            float waitTime = (introDuration < 0) ? introClip.clip.length : introDuration;
            yield return new WaitForSeconds(waitTime);
        }

        SoundClip loopClip = _bgmData.GetSoundClip(loopName);
        if (loopClip != null)
        {
            _bgmPlayer.clip = loopClip.clip;
            _bgmPlayer.volume = _globalBgmVolume * loopClip.volume;
            _bgmPlayer.loop = true;
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
            sfxPlayer.PlayOneShot(soundClip.clip, _globalSfxVolume * soundClip.volume);
            Destroy(sfxObject, soundClip.clip.length);
        }
    }

    public void PlaySFX(string name, float duration)
    {
        PlaySFX(name, 0f, duration);
    }

    public void PlaySFX_CustomStart(string name, float startTime)
    {
        SoundClip soundClip = _sfxData.GetSoundClip(name);
        if (soundClip != null && soundClip.clip != null)
        {
            float duration = soundClip.clip.length - startTime;
            if (duration > 0)
            {
                PlaySFX(name, startTime, duration);
            }
        }
    }

    public void PlaySFX(string name, float startTime, float duration)
    {
        SoundClip soundClip = _sfxData.GetSoundClip(name);
        if (soundClip != null)
        {
            StartCoroutine(CO_PlaySFXSegment(soundClip, startTime, duration));
        }
    }

    private IEnumerator CO_PlaySFXSegment(SoundClip soundClip, float startTime, float duration)
    {
        GameObject sfxObject = new GameObject("SFXPlayer_Segment");
        AudioSource sfxPlayer = sfxObject.AddComponent<AudioSource>();

        sfxPlayer.clip = soundClip.clip;
        sfxPlayer.volume = _globalSfxVolume * soundClip.volume;
        sfxPlayer.time = startTime;
        sfxPlayer.Play();

        yield return new WaitForSeconds(duration);

        if (sfxPlayer != null)
        {
            sfxPlayer.Stop();
            Destroy(sfxObject);
        }
    }

    public void SetBGMVolume(float volume)
    {
        _globalBgmVolume = Mathf.Clamp01(volume);
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
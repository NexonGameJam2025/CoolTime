using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class SoundClip
{
    public string name;
    public string description;
    public AudioClip clip;
    [Range(0f, 1f)] // 인스펙터에 0~1 사이의 슬라이더를 표시
    public float volume = 1.0f; // 클립별 개별 볼륨
}

[CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObjects/SoundData", order = 1)]
public class SoundDataSO : ScriptableObject
{
    public List<SoundClip> soundClips;

    // AudioClip 대신 SoundClip 전체를 반환하도록 수정
    public SoundClip GetSoundClip(string name)
    {
        SoundClip soundClip = soundClips.FirstOrDefault(s => s.name == name);
        if (soundClip == null)
        {
            Debug.LogWarning($"Sound clip '{name}' not found in {this.name}!");
            return null;
        }
        return soundClip;
    }
}
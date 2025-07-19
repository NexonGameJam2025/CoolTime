using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class SoundClip
{
    public string name;
    public string description;
    public AudioClip clip;
    [Range(0f, 1f)] // �ν����Ϳ� 0~1 ������ �����̴��� ǥ��
    public float volume = 1.0f; // Ŭ���� ���� ����
}

[CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObjects/SoundData", order = 1)]
public class SoundDataSO : ScriptableObject
{
    public List<SoundClip> soundClips;

    // AudioClip ��� SoundClip ��ü�� ��ȯ�ϵ��� ����
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
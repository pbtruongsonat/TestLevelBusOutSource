using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/AudioClipRefsSO", fileName = "AudioClipRefsSO")]
public class AudioClipRefsSO : ScriptableObject
{
    public List<Sound> keySounds;
    public List<Music> keyMusics;
}

[System.Serializable]
public class Sound
{
    public KeySound eSound;
    public AudioClip audioClip;
}

[System.Serializable]
public class Music
{
    public KeyMusic key;
    public AudioClip audioClip;
}

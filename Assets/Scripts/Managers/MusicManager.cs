using Obvious.Soap;
using UnityEngine;
using DG.Tweening;

public class MusicManager : SingletonBase<MusicManager>
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    [SerializeField] private FloatVariable volume;
    [SerializeField] private ScriptableEventNoParam OnStartLevel;

    private void Start()
    {
        OnStartLevel.OnRaised += OnStartLevel_OnRaised;
        volume.OnValueChanged += Volume_OnValueChanged;
    }

    private void Volume_OnValueChanged(float value)
    {
        if (value > 0)
        {
            PlayAndFadeMusic(KeyMusic.BGM_Ingame, true, 0.9f, 0.5f);
        }
        else
        {
            StopMusic();
        }
    }

    private void OnStartLevel_OnRaised()
    {
        PlayAndFadeMusic(KeyMusic.BGM_Ingame, true, 0.9f, 0.5f);
    }

    private void StopMusic()
    {
        audioSource.Stop();
    }

    private void PlayMusic(AudioClip audioClip, bool isLoop, float volumeMultiplier = 1f)
    {
        if (volume.Value == 0) return;

        if (audioClip == null)
        {
            StopMusic();
            return;
        }

        audioSource.clip = audioClip;
        audioSource.volume = volumeMultiplier * volume;
        audioSource.Play();
        audioSource.loop = isLoop;
    }

    public void PlayMusic(KeyMusic key, bool isLoop, float volumeMultiplier = 1f)
    {
        var audioClip = audioClipRefsSO.keyMusics.Find(x => x.key == key);

        if (audioClip == null) return;

        PlayMusic(audioClip.audioClip, isLoop, volumeMultiplier);
    }

    public void FadeVolume(float volume, float duration, float delay = 0f)
    {
        if (this.volume.Value == 0) return;

        if (audioSource.clip != null)
        {
            audioSource.DOFade(volume, duration).SetDelay(delay).SetUpdate(true);
        }
    }

    public void PlayAndFadeMusic(KeyMusic key, bool isLoop, float endVolume, float duration, float delay = 0f)
    {
        PlayMusic(key, isLoop, 0);
        FadeVolume(endVolume, duration, delay);
    }
}

public enum KeyMusic
{
    None = -1,
    BGM_Ingame = 0,
}

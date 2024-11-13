using Obvious.Soap;
using UnityEngine;

public class SoundManager : SingletonBase<SoundManager>
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    [SerializeField] private FloatVariable volume;

    private void Start()
    {
        Car.OnMoveAway += Car_OnMoveAway;
    }

    private void Car_OnMoveAway(object sender, System.EventArgs e)
    {
        PlaySound(KeySound.Coin_Cash);
    }

    private void PlaySound(AudioClip audioClip, bool isLoop = false, float volumeMultiplier = 1f)
    {
        if (volume.Value == 0) return;

        if (audioClip == null)
        {
            audioSource.Stop();
            return;
        }

        if (isLoop)
        {
            audioSource.clip = audioClip;
            audioSource.volume = volumeMultiplier * volume;
            audioSource.Play();
            audioSource.loop = isLoop;
        }
        else
        {
            audioSource.PlayOneShot(audioClip, volumeMultiplier * volume);
        }
    }

    public void PlaySound(KeySound keySound, bool isLoop = false, float volumeMultiplier = 1f)
    {
        if (keySound == KeySound.None) return;

        var audioClip = audioClipRefsSO.keySounds.Find(x => x.eSound == keySound);

        if (audioClip == null) return;

        PlaySound(audioClip.audioClip, isLoop, volumeMultiplier);
    }
}

public enum KeySound
{
    None = -1,
    Car_Hit = 0,
    Car_Run = 1,
    Guest_Enter = 2,
    Coin_Cash = 3,
    Win = 4,
    Lose = 5,
    Btn_Close = 6,
    Btn_Open = 7,
}

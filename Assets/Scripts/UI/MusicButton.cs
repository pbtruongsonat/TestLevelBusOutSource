using Obvious.Soap;
using UnityEngine;

public class MusicButton : SettingButton
{
    [SerializeField] private FloatVariable volume;

    protected override void DoToggle()
    {
        base.DoToggle();

        if (volume.Value > 0)
        {
            volume.Value = 0;
        }
        else
        {
            volume.Value = 1;
        }

        UpdateVisual();
    }

    protected override void UpdateVisual()
    {
        base.UpdateVisual();

        offObj.SetActive(volume.Value == 0);
        toggle.OnChanged(volume.Value > 0);
    }

    protected override void Tracking()
    {
        base.Tracking();

        //SonatTracking.ClickIcon(volume.Value > 0 ? "music_off" : "music_on", DataManager.Instance.playerData.saveLevelData.currentLevel,
        //    "ingame", "settings");
    }
}

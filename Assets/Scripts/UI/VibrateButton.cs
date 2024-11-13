using Obvious.Soap;
using UnityEngine;

public class VibrateButton : SettingButton
{
    [SerializeField] private BoolVariable isOn;

    protected override void DoToggle()
    {
        base.DoToggle();

        isOn.Toggle();

        UpdateVisual();
    }

    protected override void UpdateVisual()
    {
        base.UpdateVisual();

        offObj.SetActive(!isOn.Value);
        toggle.OnChanged(isOn.Value);
    }

    protected override void Tracking()
    {
        base.Tracking();

        //SonatTracking.ClickIcon(isOn.Value ? "vibrate_off" : "vibrate_on", DataManager.Instance.playerData.saveLevelData.currentLevel,
        //    "ingame", "settings");
    }
}

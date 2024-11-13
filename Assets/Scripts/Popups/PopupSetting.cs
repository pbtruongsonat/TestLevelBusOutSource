using Obvious.Soap;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : PopupBase
{
    [Header("----------Popup----------")]
    [Space]
    [SerializeField] private Button restoreBtn;
    [SerializeField] private Button policyBtn;
    [SerializeField] private Button cheatBtn;

    [SerializeField] private ScriptableEventString OnShowNotiAlert;

    public override void OnSetup()
    {
        base.OnSetup();

        cheatBtn.onClick.AddListener(CheatManager.Instance.OnClick);

        policyBtn.onClick.AddListener(() =>
        {
            //SonatTracking.ClickIcon("policy", currentLevel.Value, "ingame", "settings");
            Dismiss();
            PopupManager.Instance.QueuePush(PopupConfig.PopupPolicy);
        });

        restoreBtn.onClick.AddListener(() =>
        {
            //SonatTracking.ClickIcon("restore", currentLevel.Value, "ingame", "settings");

            //bool restoreSucess = IAPManager.Restore();

            //if (restoreSucess)
            //{
            //    OnShowNotiAlert.Raise("Restore successful!");
            //    Dismiss();
            //}
            //else
            //{
            //    OnShowNotiAlert.Raise("Restore failed!");
            //}
        });
    }

    public override void OnPush(Data data)
    {
        base.OnPush(data);

        //SonatTracking.LogShowUI("user", currentLevel.Value, "settings", "ingame", "settings", "open");
    }

    public override void OnPop()
    {
        base.OnPop();

        //SonatTracking.LogShowUI("user", currentLevel.Value, "settings", "ingame", "settings", "close");
    }
}

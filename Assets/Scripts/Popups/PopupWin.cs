using UnityEngine;
using UnityEngine.UI;

public class PopupWin : PopupBase
{
    [Header("----------Popup----------")]
    [Space]
    [SerializeField] private Button nextBtn;

    public override void OnSetup()
    {
        base.OnSetup();

        if (GameManager.Instance.eScreen != EScreen.Win)
        {
            GameManager.Instance.eScreen = EScreen.Win;
        }

        nextBtn.onClick.AddListener(OnNextLevel);
    }

    public override void OnPush(Data data)
    {
        base.OnPush(data);

        MusicManager.Instance.FadeVolume(0, 0.5f);

        //SonatTracking.LogShowUI("auto", currentLevel.Value, "win", "win", "win", "open");
    }

    public override void OnPop()
    {
        base.OnPop();

        //SonatTracking.LogShowUI("user", currentLevel.Value, "win", "win", "win", "close");
    }

    private void OnNextLevel()
    {
        //SonatTracking.ShowInterstitial(currentLevel.Value - 1, "win", "win", () =>
        //{
        DataManager.Instance.playerData.realLevel = currentLevel;
        GameplayManager.Instance.StartLevel();
        Dismiss();
        //});
    }
}

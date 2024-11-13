using UnityEngine;
using UnityEngine.UI;

public class PopupLose : PopupBase
{
    [Header("----------Popup----------")]
    [Space]
    [SerializeField] private Button replayBtn;

    public override void OnSetup()
    {
        base.OnSetup();

        if (GameManager.Instance.eScreen != EScreen.Lose)
        {
            GameManager.Instance.eScreen = EScreen.Lose;
        }

        //SonatTracking.SetCurrentScreenName(EScreen.Lose);

        replayBtn.onClick.AddListener(OnReplay);
    }

    public override void OnPush(Data data)
    {
        base.OnPush(data);

        //SonatTracking.LogShowUI("auto", currentLevel.Value, "lose", "lose", "fail", "open");
    }

    public override void OnPop()
    {
        base.OnPop();

        //SonatTracking.LogShowUI("user", currentLevel.Value, "lose", "lose", "fail", "close");
    }

    private void OnReplay()
    {
        //SonatTracking.ShowInterstitial(currentLevel.Value, "lose", "fail", () =>
        //{
            GameplayManager.Instance.Replay();
            Dismiss();

        Debug.Log("Replay");
        //});
    }
}

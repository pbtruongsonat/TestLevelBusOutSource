using Obvious.Soap;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class PopupOutOfSpace : PopupBase
{
    [Header("----------Popup----------")]
    [Space]
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button adsBtn;

    [SerializeField] private IntVariable coin;
    [SerializeField] private IntVariable numContinue;

    private bool isRevive;

    public override void OnSetup()
    {
        base.OnSetup();


        if (GameManager.Instance.eScreen != EScreen.Lose)
        {
            GameManager.Instance.eScreen = EScreen.Lose;
        }



        costText.text = "<sprite name=coin> " + GameDefine.COIN_REVIVE;

        buyBtn.onClick.AddListener(() =>
        {
            if (coin.Value >= GameDefine.COIN_REVIVE)
            {
                coin.Add(-GameDefine.COIN_REVIVE);

                //SonatTracking.LogSpendCurrency("coin", "currency", GameDefine.COIN_REVIVE, "out_of_space", "lose", "booster", "add_parking_lot");

                isRevive = true;
                ParkingLotManager.Instance.UnlockNextSpace();
                Dismiss();
            }
            else
            {
                PopupManager.Instance.QueuePush(PopupConfig.PopupShop);
            }        
        });

        adsBtn.onClick.AddListener(() =>
        {
            //SonatTracking.PlayVideoAds("add_parking_lot", () =>
            //{
                isRevive = true;
                ParkingLotManager.Instance.UnlockNextSpace();
                Dismiss();
            //}, "booster", "lose", "out_of_space");
        });
    }

    public override void OnPush(Data data)
    {
        base.OnPush(data);

        MusicManager.Instance.FadeVolume(0, 0.5f);

        isRevive = false;

        //SonatTracking.LogShowUI("auto", currentLevel.Value, "out_of_space", "lose", "out_of_space", "open");
    }

    public override void OnPop()
    {
        base.OnPop();

        //SonatTracking.LogShowUI("auto", currentLevel.Value, "out_of_space", "lose", "out_of_space", "close");
    }

    public override void Dismiss()
    {
        base.Dismiss();

        if (!isRevive)
        {
            PopupManager.Instance.QueuePush(PopupConfig.PopupLose);
        }
        else
        {
            if (GameManager.Instance.eScreen != EScreen.Ingame)
            {
                GameManager.Instance.eScreen = EScreen.Ingame;
            }


            MusicManager.Instance.FadeVolume(1, 0.5f);

            numContinue.Add(1);

            //SonatTracking.LogLevelStart(currentLevel.Value, DataManager.Instance.IsFirstPlay(),
            //    Sonat.StartLevelType.Revive, "add_parking_lot", numContinue.Value);
        }
    }
}

using Obvious.Soap;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupBooster : PopupBase
{
    [Header("----------Popup----------")]
    [Space]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image boosterIcon;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button adsBtn;

    [SerializeField] private BoosterSO boosterSO;

    [SerializeField] private IntVariable coin;

    private BoosterInfo info;

    public override void OnSetup()
    {
        base.OnSetup();

        buyBtn.onClick.AddListener(() =>
        {
            if (coin.Value >= info.cost)
            {
                coin.Add(-info.cost);

                //SonatTracking.LogSpendCurrency("coin", "currency", info.cost, "ingame", info.type.ToString(), "booster", info.type.ToString());

                BoosterManager.Instance.AddBooster(info.type, 1);
                Dismiss();
            }
            else
            {
                Data data = new Data();
                data.Add("isAuto", true);

                PopupManager.Instance.QueuePush(PopupConfig.PopupShop, data);
            }
        });

        adsBtn.onClick.AddListener(() =>
        {
            //SonatTracking.PlayVideoAds(info.type.ToString(), () =>
            //{
                BoosterManager.Instance.AddBooster(info.type, 1);
                Dismiss();
            //}, "booster", "ingame", info.type.ToString());
        });
    }

    public override void OnPush(Data data)
    {
        base.OnPush(data);

        if (data != null)
        {
            Init((BoosterType)data.Get("boosterType"));
        }

        //SonatTracking.LogShowUI("user", currentLevel.Value, "buy_booster", "ingame", info.type.ToString(), "open");
    }

    public override void OnPop()
    {
        base.OnPop();

        //SonatTracking.LogShowUI("user", currentLevel.Value, "buy_booster", "ingame", info.type.ToString(), "close");
    }

    private void Init(BoosterType type)
    {
        BoosterInfo info = boosterSO.GetBoosterInfo(type);

        if (info == null)
        {
            Debug.LogError("Cant find info of booster " + type);
            return;
        }

        this.info = info;

        titleText.text = info.name;
        descriptionText.text = info.description;
        costText.text = "<sprite name=coin> " + info.cost;
        boosterIcon.sprite = info.bigIcon;
        boosterIcon.SetNativeSize();
    }
}

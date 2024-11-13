using Obvious.Soap;
using System.Collections.Generic;
using UnityEngine;

public class PopupShop : PopupBase
{
    [Header("----------Popup----------")]
    [Space]
    [SerializeField] private List<GameObject> noAds;

    [SerializeField] private BoolVariable isNoAds;

    private void OnEnable()
    {
        ActiveNoAds(isNoAds.Value);

        isNoAds.OnValueChanged += ActiveNoAds;
    }

    private void OnDisable()
    {
        isNoAds.OnValueChanged -= ActiveNoAds;
    }

    private void ActiveNoAds(bool value)
    {
        noAds.SetActiveAll(!value);
    }

    public override void OnPush(Data data)
    {
        base.OnPush(data);

        if (data != null)
        {
            isAuto = (bool)data.Get("isAuto");
        }

        //SonatTracking.LogShowUI(isAuto ? "auto" : "user", currentLevel.Value, "shop_iap", "ingame", "shop_iap", "open");
    }

    public override void OnPop()
    {
        base.OnPop();

        //SonatTracking.LogShowUI(isAuto ? "auto" : "user", currentLevel.Value, "shop_iap", "ingame", "shop_iap", "close");
    }
}

using Obvious.Soap;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class IAPManager
{
    public static ShopItemKey packClicked;
    public static ShopItemKey packBuySuccess = ShopItemKey.None;

    public static Action OnPurchasedFailedAction;

    public static void Buy(ShopItemKey shopItemKey, Action onSuccess, string screen)
    {
        packClicked = shopItemKey;

        //string productId = GetProductID(shopItemKey);

        //Sonat.SonatLogBuyShopItemIapInput log = new Sonat.SonatLogBuyShopItemIapInput(productId, "pack", 1)
        //{
        //    location = "ingame",
        //    screen = screen,
        //};

        //Kernel.Resolve<BasePurchaser>().Buy((int)shopItemKey, success =>
        //{
        //    if (success)
        //    {
        //        packBuySuccess = shopItemKey;

        //        onSuccess?.Invoke();

        //        if (shopItemKey == ShopItemKey.NoAds)
        //        {
        //            DataManager.Instance.EnableNoAds();
        //            DataManager.Instance.RestorePack(shopItemKey);
        //        }
        //    }
        //    else
        //    {
                OnPurchasedFailedAction?.Invoke();
        //    }
        //}, log);
    }

    //public static string GetProductID(ShopItemKey key)
    //{
    //    //if (!Kernel.Resolve<BasePurchaser>().IsInitialized()) return "";
    //    //var shopItem = Kernel.Resolve<BasePurchaser>().StoreProductDescriptors.Find(x => x.key == (int)key);
    //    return shopItem?.StoreProductId;
    //}

    public static void Claim(List<Reward> rewards, int level, string source, string screen, string placement)
    {
        for (int i = 0; i < rewards.Count; i++)
        {
            Claim(rewards[i], level, source, screen, placement);
        }
    }

    public static void Claim(Reward reward, int level, string source, string screen, string placement)
    {      
        switch (reward.id)
        {
            case RewardID.noAds:
                DataManager.Instance.EnableNoAds();
                break;
            //case RewardID.unlimitedLives:
            //    DataManager.Instance.SetUnlimitedLives((int)reward.time, false);

            //    SonatTracking.LogEarnCurrency("unlimitedLives", "other_source", (int)reward.time, screen, placement,
            //        source, level);
            //    break;
            //case RewardID.live:
            //    DataManager.Instance.AddLives(reward.amount, false);

            //    SonatTracking.LogEarnCurrency("live", "other_source", reward.amount, screen, placement,
            //        source, level);
            //    break;
            case RewardID.coin:
                DataManager.Instance.ChangeCoin(reward.amount);

                //SonatTracking.LogEarnCurrency("coin", "currency", reward.amount, screen, placement,
                //    source, level);
                break;
            //case RewardID.hammer:
            //    BoosterManager.Instance.AddBooster(EBoosterType.Hammer, reward.amount, false);

            //    SonatTracking.LogEarnCurrency("hammer", "booster", reward.amount, screen, placement,
            //        source, level);
            //    break;
            //case RewardID.addHole:
            //    BoosterManager.Instance.AddBooster(EBoosterType.AddHole, reward.amount, false);

            //    SonatTracking.LogEarnCurrency("addHole", "booster", reward.amount, screen, placement,
            //        source, level);
            //    break;
            //case RewardID.addBox:
            //    BoosterManager.Instance.AddBooster(EBoosterType.AddBox, reward.amount, false);

            //    SonatTracking.LogEarnCurrency("addBox", "booster", reward.amount, screen, placement,
            //        source, level);
            //    break;
        }
    }

//    public static bool Restore()
//    {
//        Debug.Log("abc start restore");

//#if UNITY_ANDROID     
//        return MyRestore();
//#elif UNITY_IOS
//        Kernel.Resolve<BasePurchaser>().RestorePurchasesIOS(() =>
//        {
//            MyRestore();

//            callback?.Invoke();
//        });
//#endif
//    }

    //private static bool MyRestore()
    //{
    //    if (Helper.IsPurchaserInitFailed()) return false;

    //    bool isSuccess = false;

    //    if (Kernel.Resolve<BasePurchaser>().CheckHasPurchasedProductId((int)ShopItemKey.NoAds)
    //        && !Kernel.Resolve<AdsManager>().IsNoAds())
    //    {
    //        DataManager.Instance.EnableNoAds();
    //        DataManager.Instance.RestorePack(ShopItemKey.NoAds);

    //        isSuccess = true;
    //    }

    //    return isSuccess;
    ////}
}

public enum ShopItemKey
{
    None = -1,
    NoAds = 0,
    Coin1 = 1,
    Coin2 = 2,
    Coin3 = 3,
    Coin4 = 4,
    Coin5 = 5,
    Coin6 = 6,
}

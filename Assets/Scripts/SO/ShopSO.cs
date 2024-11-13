using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/ShopSO", fileName = "ShopSO")]
public class ShopSO : ScriptableObject
{
    public List<ShopPack> packs;

    public ShopPack GetShopPack(ShopItemKey key)
    {
        return packs.Find(x => x.key == key);
    }
}

[Serializable]
public class ShopPack
{
    public string name;
    public List<Reward> rewards;
    public float costValue;
    public int limitBuyCount;
    public Sprite packIcon;
    public ShopItemKey key;

    public int GetRewardAmount(RewardID rewardType)
    {
        foreach (Reward reward in rewards)
        {
            if (reward.id == rewardType)
            {
                return reward.amount;
            }
        }
        return 0;
    }
}

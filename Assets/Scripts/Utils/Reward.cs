using System;

[Serializable]
public class Reward
{
    //public int id;
    public RewardID id;
    public RewardType type;
    public int amount;
    public float time;

    public Reward(RewardID rewardID, RewardType rewardType, int amount)
    {
        this.id = rewardID;
        this.type = rewardType;
        this.amount = amount;
        this.time = 0;
    }

    public Reward(RewardID rewardID, RewardType rewardType, float time)
    {
        this.id = rewardID;
        this.type = rewardType;
        this.time = time;
        this.amount = 0;
    }

    public Reward(RewardID rewardID, RewardType rewardType, int amount, float time)
    {
        this.id = rewardID;
        this.type = rewardType;
        this.time = time;
        this.amount = amount;
    }

    public Reward()
    {

    }
}

public enum RewardID
{
    noAds = 0,
    noAds24h = 1,
    coin = 2,
    live = 3,
    unlimitedLives = 4,
    hammer = 5,
    addHole = 6,
    addBox = 7,
}

public enum RewardType
{
    noAds,
    booster,
    currency,
}

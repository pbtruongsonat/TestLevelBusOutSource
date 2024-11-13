using TMPro;
using UnityEngine;

public class UICoinPack : UIShopPack
{
    [SerializeField] private TextMeshProUGUI valueText;

    public override void Init()
    {
        base.Init();

        packIcon.sprite = data.packIcon;

        foreach (Reward reward in data.rewards)
        {
            if (reward.id == RewardID.coin)
            {
                valueText.text = reward.amount.ToString();
                break;
            }
        }
    }
}

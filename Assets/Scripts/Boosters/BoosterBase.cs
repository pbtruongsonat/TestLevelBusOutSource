using Obvious.Soap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterBase : MonoBehaviour
{
    [SerializeField] protected Button button;
    [SerializeField] protected BoosterType type;
    [SerializeField] protected ToggleScript toggleState;
    [SerializeField] protected TextMeshProUGUI amountText;
    [SerializeField] protected GameObject plusObj;

    [SerializeField] private IntVariable useBoosterCount;
    [SerializeField] private IntVariable currentLevel;

    protected int amount;

    public BoosterType Type => type;

    public virtual void Use()
    {
        amount--;
        amount = Mathf.Max(amount, 0);
        amountText.SetText(amount.ToString());

        useBoosterCount.Add(1);

        Save();

        CheckOutOfBooster();

        //SonatTracking.LogUseBoosterUA();
        //SonatTracking.LogUseBooster(currentLevel.Value, type.ToString());
        //SonatTracking.LogSpendCurrency(type.ToString(), "booster", 1, "ingame", "ingame");
    }

    public virtual void LoadData()
    {
        var boosters = DataManager.Instance.playerData.boosters;

        foreach (var booster in boosters)
        {
            if (booster.type == type)
            {
                amount = booster.amount;
                break;
            }
        }

        CheckOutOfBooster();
    }

    public virtual void CheckOutOfBooster()
    {
        button.onClick.RemoveAllListeners();

        if (amount > 0)
        {
            toggleState.OnChanged(true);
            plusObj.SetActive(false);

            amountText.gameObject.SetActive(true);
            amountText.text = amount.ToString();

            button.onClick.AddListener(Use);
        }
        else
        {
            toggleState.OnChanged(false);
            plusObj.SetActive(true);
            amountText.gameObject.SetActive(false);

            button.onClick.AddListener(() =>
            {
                var uiData = new View.Data();
                uiData.Add("boosterType", type);

                PopupManager.Instance.QueuePush(PopupConfig.PopupBooster, uiData);
            });
        }
    }

    private void Save()
    {
        foreach (Booster booster in DataManager.Instance.playerData.boosters)
        {
            if (booster.type == type)
            {
                booster.amount = amount;
                DataManager.Instance.Save();
            }
        }
    }

    public virtual void Add(int value, bool isLog = true)
    {
        var boosters = DataManager.Instance.playerData.boosters;

        foreach (var booster in boosters)
        {
            if (booster.type == type)
            {
                amount = booster.amount;
                break;
            }
        }

        amount += value;
        amountText.text = amount.ToString();

        Save();

        CheckOutOfBooster();

        //if (isLog)
        //{
        //    SonatTracking.LogEarnCurrency(type.ToString(), "booster", value, "ingame", type.ToString(), "non_iap", currentLevel.Value);
        //}
    }
}

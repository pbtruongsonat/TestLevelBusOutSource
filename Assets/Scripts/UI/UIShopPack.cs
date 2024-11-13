using Obvious.Soap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopPack : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] protected Image packIcon;
    [SerializeField] private Button buyBtn;

    [SerializeField] private ShopSO shopSO;

    [SerializeField] protected ScriptableEventString OnShowNotiAlert;

    public ShopItemKey key;

    protected ShopPack data;

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        data = shopSO.GetShopPack(key);

        costText.text = "$" + data.costValue;
        buyBtn.onClick.AddListener(() =>
        {
            Debug.Log("click buy " + key.ToString());

            //if (Kernel.IsInternetConnection())
            //{

            //    IAPManager.Buy(key, () =>
            //    {
            //        OnBuySuccess();
            //    }, "shop_iap");
            //}
            //else
            //{
            //    OnShowNotiAlert.Raise("No internet connection!");
            //}
        });
    }

    protected virtual void OnBuySuccess()
    {
        IAPManager.Claim(data.rewards, DataManager.Instance.playerData.saveLevelData.currentLevel, "iap", "ingame", "shop_iap");
    }
}

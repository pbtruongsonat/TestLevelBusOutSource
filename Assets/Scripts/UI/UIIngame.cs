using Obvious.Soap;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIngame : MonoBehaviour
{
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button shopBtn;
    [SerializeField] private Button noAdsBtn;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private List<GameObject> objects;

    [SerializeField] private IntVariable currentLevel;
    [SerializeField] private BoolVariable isNoAds;
    [SerializeField] private ScriptableEventNoParam OnStartLevel;
    [SerializeField] private ScriptableEventString OnShowNotiAlert;

    private void Awake()
    {
        shopBtn.onClick.AddListener(() =>
        {
            PopupManager.Instance.QueuePush(PopupConfig.PopupShop);
        });

        replayBtn.onClick.AddListener(() =>
        {
            //SonatTracking.ShowInterstitial(currentLevel.Value, "ingame", "ingame", () =>
            //{
                GameplayManager.Instance.Replay();
            //});
        });

        settingBtn.onClick.AddListener(() =>
        {
            PopupManager.Instance.QueuePush(PopupConfig.PopupSetting);
        });

        noAdsBtn.onClick.AddListener(() =>
        {
            //if (Kernel.IsInternetConnection())
            //{
            //    IAPManager.Buy(ShopItemKey.NoAds, () =>
            //    {

            //    }, "ingame");
            //}
            //else
            //{
            //    OnShowNotiAlert.Raise("No internet connection!");
            //}
        });
    }

    private void OnEnable()
    {
        OnStartLevel.OnRaised += OnStartLevel_OnRaised; 
        isNoAds.OnValueChanged += ActiveNoAds;
    }

    private void OnDisable()
    {
        OnStartLevel.OnRaised -= UpdateLevelText;
        isNoAds.OnValueChanged -= ActiveNoAds;
    }

    private void OnStartLevel_OnRaised()
    {
        UpdateLevelText();
        ActiveUI(currentLevel.Value > 1);
    }

    private void UpdateLevelText()
    {
        levelText.text = "Level " + currentLevel.Value;
    }

    private void ActiveNoAds(bool value)
    {
        noAdsBtn.gameObject.SetActive(!value);
    }

    private void ActiveUI(bool isActive)
    {
        objects.SetActiveAll(isActive);

        if (isNoAds.Value)
        {
            noAdsBtn.gameObject.SetActive(false);
        }
        else
        {
            noAdsBtn.gameObject.SetActive(currentLevel.Value > 1);
        }
    }
}

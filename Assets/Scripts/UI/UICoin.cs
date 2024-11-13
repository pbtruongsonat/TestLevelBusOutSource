using Obvious.Soap;
using UnityEngine;

public class UICoin : UICurrency
{
    [SerializeField] private IntVariable coin;
    [SerializeField] private IntVariable currentLevel;
    [SerializeField] private ScriptableEventNoParam OnStartLevel;

    private void OnEnable()
    {
        valueText.text = coin.Value.ToString();

        coin.OnValueChanged += UpdateCoin;
        OnStartLevel.OnRaised += OnStartLevel_OnRaised;
    }

    private void OnDisable()
    {
        coin.OnValueChanged -= UpdateCoin;
        OnStartLevel.OnRaised -= OnStartLevel_OnRaised;
    }

    private void UpdateCoin(int value)
    {
        TweenText(coin.PreviousValue, value, timeTween);
    }

    protected override void DoMoreAction()
    {
        base.DoMoreAction();

        if (currentLevel.Value == 1) return;

        PopupManager.Instance.QueuePush(PopupConfig.PopupShop);
    }

    private void OnStartLevel_OnRaised()
    {
        //ActiveMoreIcon(currentLevel.Value > 1);
    }
}

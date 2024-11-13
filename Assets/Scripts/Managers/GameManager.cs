using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    [SerializeField] private GameStateEnum gameState;

    public string location;
    public EScreen eScreen;

    private void Start()
    {
        gameState.Value = GameState.None;

        gameState.OnValueChanged += GameState_OnValueChanged;

        StartCoroutine(IeInit());
    }

    private void GameState_OnValueChanged(GameState obj)
    {
        if (obj == GameState.Ingame)
        {
            location = "ingame";
            eScreen = EScreen.Ingame;
        }
    }

    private IEnumerator IeInit()
    {
#if UNITY_ANDROID
        IAPManager.Restore();
#endif
        gameState.Value = GameState.Ingame;

        yield return new WaitForSeconds(0.2f);

        //Kernel.Resolve<AdsManager>().ShowBanner();
    }
}

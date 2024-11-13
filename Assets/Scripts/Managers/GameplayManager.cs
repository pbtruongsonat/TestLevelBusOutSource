using DG.Tweening;
using Obvious.Soap;
using UnityEngine;

public class GameplayManager : SingletonBase<GameplayManager>
{
    [SerializeField] private Camera cam;

    [SerializeField] private BoolVariable isSelectingVipCar;
    [SerializeField] private BoolVariable isUsingMagnet;
    [SerializeField] private BoolVariable isPauseGame;
    [SerializeField] private BoolVariable isBlockInteract;
    [SerializeField] private GameStateEnum gameState;
    [SerializeField] private IntVariable currentLevel;
    [SerializeField] private IntVariable numContinue;
    [SerializeField] private IntVariable useBoosterCount;
    [SerializeField] private FloatVariable timePlay;
    [SerializeField] private ScriptableEventNoParam OnStartLevel;

    public LayerMask touchLayer;
    public LayerMask carLayer;
    public LayerMask obstacleLayer;

    private bool isOver;

    private void Awake()
    {
        if (SystemInfo.graphicsMemorySize <= 1536)
            Application.targetFrameRate = 30;
        else
            Application.targetFrameRate = 60;

        Input.multiTouchEnabled = false;
    }

    private void Start()
    {
        gameState.OnValueChanged += GameState_OnValueChanged;
        isPauseGame.OnValueChanged += IsPauseGame_OnValueChanged;
    }

    private void IsPauseGame_OnValueChanged(bool value)
    {
        if (isOver)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = value ? 0 : 1;
        }
    }

    private void GameState_OnValueChanged(GameState state)
    {
        if (state == GameState.Ingame)
        {
            StartLevel();
        }
    }

    public void StartLevel()
    {
        GameManager.Instance.eScreen = EScreen.Ingame;

        ResetData();

        DataManager.Instance.LoadCurLevel();

        OnStartLevel.Raise();
    }

    private void ResetData()
    {
        isSelectingVipCar.Value = false;
        isPauseGame.Value = false;
        isOver = false;

        numContinue.Value = 0;
        useBoosterCount.Value = 0;
    }

    private void Update()
    {
        if (!isPauseGame.Value)
        {
            timePlay.Add(Time.deltaTime);
        }

        if (isPauseGame.Value || isBlockInteract.Value) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(mousePosition, touchLayer);

            if (collider == null) return;

            if (collider.transform.parent.TryGetComponent(out Car car))
            {
                if (isSelectingVipCar.Value)
                {
                    isBlockInteract.Value = true;

                    car.MoveToVipParking(() =>
                    {
                        isSelectingVipCar.Value = false;
                        isBlockInteract.Value = false;
                    });
                }
                else if (isUsingMagnet.Value)
                {
                    isBlockInteract.Value = true;

                    car.MoveToNormalParking(() =>
                    {
                        isUsingMagnet.Value = false;
                        isBlockInteract.Value = false;
                    });
                }
                else
                {
                    car.OnClick();
                }

                if (!ParkingLotManager.Instance.HasPossibleClickCar())
                    isBlockInteract.Value = true;

                EffectManager.Instance.ShowClickCarEffect(mousePosition);
            }
            else if (collider.transform.parent.TryGetComponent(out ParkingSpace parkingSpace))
            {
                if (isSelectingVipCar.Value) return;

                //SonatTracking.PlayVideoAds("add_parking_lot", () =>
                //{
                    parkingSpace.Unlock();

                    //SonatTracking.LogEarnCurrency("add_parking_lot", "booster", 1, "ingame", "ingame", "non_iap", currentLevel.Value);
                    //SonatTracking.LogUseBooster(currentLevel.Value, "add_parking_lot");
                    //SonatTracking.LogSpendCurrency("add_parking_lot", "booster", 1, "ingame", "ingame");

                    ParkingLotManager.Instance.StopWarningAll();

                //}, "booster", "ingame", "ingame");
            }
        }
    }

#if use_winlose
    public void Win()
    {
        //SonatTracking.LogLevelEnd(currentLevel.Value,
        //    useBoosterCount.Value,
        //     true,
        //    (int)timePlay.Value,
        //    DataManager.Instance.IsFirstPlay(),
        //    "",
        //    0,
        //    0,
        //    numContinue.Value == 0 ? "" : "add_parking_lot",
        //    numContinue.Value
        //    );

        //SonatTracking.LogCompleteLevelUA(currentLevel.Value);
        //SonatTracking.LogLevelUp(currentLevel.Value + 1);

        isBlockInteract.Value = true;
        currentLevel.Value++;
        isOver = true;
        isSelectingVipCar.Value = false;
    }

    public void ShowWin()
    {
        SoundManager.Instance.PlaySound(KeySound.Win);

        isBlockInteract.Value = false;

        PopupManager.Instance.QueuePopAll();
        PopupManager.Instance.QueuePush(PopupConfig.PopupWin);
    }

    public void Lose()
    {
        isBlockInteract.Value = true;
        isOver = true;
        isSelectingVipCar.Value = false;

        //SonatTracking.LogLevelEnd(currentLevel.Value,
        //    useBoosterCount.Value,
        //     false,
        //    (int)timePlay.Value,
        //    DataManager.Instance.IsFirstPlay(),
        //    "out_of_space",
        //    0,
        //    0,
        //    numContinue.Value == 0 ? "" : "add_parking_lot",
        //    numContinue.Value
        //    );

        DOVirtual.DelayedCall(0.5f, () =>
        {
            SoundManager.Instance.PlaySound(KeySound.Lose);

            isBlockInteract.Value = false;

            PopupManager.Instance.QueuePopAll();

            if (ParkingLotManager.Instance.IsMaxSpace())
            {
                PopupManager.Instance.QueuePush(PopupConfig.PopupLose);
            }
            else
            {
                PopupManager.Instance.QueuePush(PopupConfig.PopupOutOfSpace);
            }
        });
    }

#endif
    public void Replay()
    {
        //SonatTracking.LogLevelEnd(currentLevel.Value,
        //    useBoosterCount.Value,
        //     false,
        //    (int)timePlay.Value,
        //    DataManager.Instance.IsFirstPlay(),
        //    "replay",
        //    0,
        //    0,
        //    numContinue.Value == 0 ? "" : "add_parking_lot",
        //    numContinue.Value
        //    );

        StartLevel();
    }
}

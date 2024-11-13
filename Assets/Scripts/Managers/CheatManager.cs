using DG.Tweening;
using Obvious.Soap;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheatManager : SingletonBase<CheatManager>
{
    public TMP_InputField levelInput;
    public TMP_InputField coinInput;

    public GameObject cheatPanel;

    private int count = 0;

    [SerializeField] private GameObject panelPassword;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private GameObject cheatShow;
    [SerializeField] private Toggle toggleTurn;
    [SerializeField] private BgThemeSO bgThemeSO;

    [Header("Ingame theme")]
    [Space]
    [SerializeField] private SpriteRenderer root;
    [SerializeField] private SpriteRenderer rootCover;
    [SerializeField] private SpriteRenderer sign;
    [SerializeField] private List<TMP_Text> texts;

    [Header("Custom Path")]
    [SerializeField] private TMP_InputField inputPath;
    [SerializeField] private Button useCustomPathBtn;

    [Header("Soap")]
    [Space]
    [SerializeField] private IntVariable coin;
    [SerializeField] private IntVariable currentLevel;

    private BgTheme currentTheme;

    public PlayerData playerData => DataManager.Instance.playerData;

    private void Awake()
    {
        toggleTurn.onValueChanged.AddListener((state) =>
        {
            Turn(state);
        });

        cheatPanel.SetActive(true);
    }

    private void Start()
    {
        //currentTheme = (BgTheme)RemoteConfigHelper.GetValueInt("ingame_bg_theme");
        LoadBgIngame(currentTheme);

        inputPath.onValueChanged.AddListener((state) => { });
        useCustomPathBtn.onClick.AddListener(() => ChangeCustomPath(inputPath.text));
    }

    public void DebugTracking()
    {
        //Kernel.kernel.AddOnScreenDebugLog();
    }

    public void OnClick()
    {
        Debug.Log("1");

        if (cheatPanel.activeSelf)
        {
            cheatPanel.SetActive(false);

            return;
        }

        if (count == 0)
        {
            DOVirtual.DelayedCall(3f, () =>
            {
                count = 0;
            });
        }

        count++;

        if (count == 6)
        {
            panelPassword.SetActive(true);
        }
    }

    public void ConfirmButton()
    {
        if (passwordInput.text == "sonat@123")
        {
            cheatPanel.SetActive(true);
        }

        panelPassword.SetActive(false);

    }

    public void ClosePanel()
    {
        cheatPanel.SetActive(false);
    }

    public void Turn(bool isOn)
    {
        cheatShow.SetActive(isOn);
    }

    public void Win()
    {
        //GameplayManager.Instance.Win();
    }

    public void Lose()
    {
        //GameplayManager.Instance.IsLose = true;
        //GameplayManager.Instance.Lose();
    }

    public void NextLevel()
    {
        playerData.saveLevelData.playCount = 0;
        playerData.preLevel = playerData.nextLevel;
        currentLevel.Value++;
        playerData.realLevel = currentLevel;

        GameplayManager.Instance.Replay();
    }

    public void BackLevel()
    {
        playerData.saveLevelData.playCount = 0;
        playerData.preLevel = playerData.nextLevel;
        currentLevel.Value--;
        playerData.realLevel = currentLevel;

        GameplayManager.Instance.Replay();
    }

    public void SetLevel()
    {
        int level;

        if (int.TryParse(levelInput.text, out level))
        {
            playerData.saveLevelData.playCount = 0;
            playerData.preLevel = playerData.nextLevel;
            currentLevel.Value = level;
            playerData.realLevel = currentLevel;

            GameplayManager.Instance.Replay();
        }
    }

    public void SetCoin()
    {
        int coin;

        if (int.TryParse(coinInput.text, out coin))
        {
            this.coin.Value = coin;
        }
    }

    public void ChangeBgIngame()
    {
        if ((int)(currentTheme + 1) >= Enum.GetNames(typeof(BgTheme)).Length - 1)
        {
            currentTheme = 0;
        }
        else
        {
            currentTheme++;
        }

        LoadBgIngame(currentTheme);
    }

    private void LoadBgIngame(BgTheme bgTheme)
    {
        var themeData = bgThemeSO.GetIngameTheme(currentTheme);

        root.sprite = themeData.root;
        rootCover.sprite = themeData.rootCover;
        sign.sprite = themeData.sign;

        foreach (var text in texts)
        {
            text.color = themeData.textColor;
        }
    }

    private void ChangeCustomPath(string path)
    {
        DataManager.Instance.SetLevelPathCustom(path);

        if (DataManager.Instance.usePathCustom) {
            useCustomPathBtn.image.color = Color.green;
            useCustomPathBtn.GetComponentInChildren<TextMeshProUGUI>().text = "v";
        } 
        else
        {
            useCustomPathBtn.image.color = Color.white;
            useCustomPathBtn.GetComponentInChildren<TextMeshProUGUI>().text = "x";
        }
    }
}

using Obvious.Soap;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class DataManager : SingletonBase<DataManager>
{
    public PlayerData playerData = new PlayerData();
    public LevelData levelData = new LevelData();

    [SerializeField] private IntVariable currentLevel;
    [SerializeField] private IntVariable coin;
    [SerializeField] private BoolVariable isNoAds;
    [SerializeField] private StringVariable currentVersion;

    public string levelPathCustom = "";
    public bool usePathCustom;

    private const string LEVEL_FILE_PATH = "Levels/";
    private const string PLAYERDATA_KEY = "PlayerData";

    public PlayerData SavePlayerData
    {
        get
        {
            if (PlayerPrefs.HasKey(PLAYERDATA_KEY))
            {
                return JsonConvert.DeserializeObject<PlayerData>(PlayerPrefs.GetString(PLAYERDATA_KEY));
            }
            else
            {
                var newDataPlayer = InitPlayerData();

                SavePlayerData = newDataPlayer;

                return newDataPlayer;
            }
        }
        set
        {
            if (value == null)
                PlayerPrefs.DeleteKey(PLAYERDATA_KEY);
            else
            {
                PlayerPrefs.SetString(PLAYERDATA_KEY, JsonConvert.SerializeObject(value));
            }

        }
    }

    private PlayerData InitPlayerData()
    {
        var newDataPlayer = new PlayerData()
        {
            saveLevelData = new Level()
            {
                currentLevel = 1,
                playCount = 0,
            },

            timeFirstOpen = DateTime.Now.DateTimeToString(),

            coins = GameDefine.DEFAULT_COIN,     

            boosters = new List<Booster>()
            {
                new Booster(BoosterType.SortingGuest, 1, ""),
                new Booster(BoosterType.ShuffleCar, 1, ""),
                new Booster(BoosterType.ParkingVip, 1, ""),
                new Booster(BoosterType.Magnet, 1, ""),
            },
        };

        return newDataPlayer;
    }

    public void Save()
    {
        SavePlayerData = playerData;
    }

    private void Awake()
    {
        string levelDirectory = Application.dataPath + "/Levels/";
        if (!Directory.Exists(levelDirectory))
        {
            Directory.CreateDirectory(levelDirectory);
        }

        playerData = SavePlayerData;

        currentLevel.Value = playerData.saveLevelData.currentLevel;
        coin.Value = playerData.coins;
        //isNoAds.Value = Kernel.Resolve<AdsManager>().IsNoAds();

        CountLevel();
        CheckVersion();
    }

    private void Start()
    {
        coin.OnValueChanged += UpdateCoin;
        currentLevel.OnValueChanged += SaveLevel;
    }

    private void SaveLevel(int value)
    {
        playerData.saveLevelData.currentLevel = value;
        Save();
    }

    private void UpdateCoin(int value)
    {
        if (value == 0) return;

        playerData.coins = value;
        Save();
    }

    public void ChangeCoin(int value)
    {
        coin.Value += value;
    }

    private void CheckVersion()
    {
        currentVersion.Value = Application.version;
        Debug.Log("abc " + currentVersion.Value);
    }

    private void CountLevel()
    {
        UnityEngine.Object[] assets = Resources.LoadAll("Levels");

        GameDefine.MAX_LEVEL = 1000000;
    }

    public void LoadCurLevel()
    {
        int level = playerData.realLevel;

        //if (IsFirstPlay())
        //{
        //    SonatTracking.LogStartLevelUA(currentLevel.Value);
        //    SonatTracking.LogStartLevelUA(currentLevel.Value);

        //    SonatTracking.LogLevelStart(currentLevel.Value, true, Sonat.StartLevelType.New);
        //}
        //else
        //{
        //    SonatTracking.LogLevelStart(currentLevel.Value, false, Sonat.StartLevelType.New);
        //}

        if (GetLevelTest(level.ToString()) == null)
        {
            Debug.LogError("Not have Level: " + level);

            bool isReplay = false;

            if (isReplay)
            {
                LoadLevel(playerData.nextLevel);
            }
            else
            {
                int partLevel = (int)Math.Round(GameDefine.MAX_LEVEL / 2 / 10.0) * 10;
                int randomLevel = partLevel + currentLevel % partLevel;

                playerData.nextLevel = randomLevel;

                LoadLevel(randomLevel);

                Save();
            }

            return;
        }

        playerData.nextLevel = currentLevel.Value;
        Save();

        LoadLevel(level);
    }

    private LevelData GetLevelDefault(string levelStr)
    {
        var level = int.Parse(levelStr);

        if (level <= 0)
        {
            level = 1;
        }

        string filePath = LEVEL_FILE_PATH + level;

        TextAsset textAsset = Resources.Load<TextAsset>(filePath);

        string jsonContent = textAsset.ToString();

        return JsonConvert.DeserializeObject<LevelData>(jsonContent);
    }

    private LevelData GetLevelTest(string levelID)
    {
        string path;
        if (usePathCustom)
        {
            path = levelPathCustom + levelID + ".json";
        }
        else
        {
            path = Application.dataPath + "/Levels/" + levelID + ".json";
        }

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LevelData levelData = JsonConvert.DeserializeObject<LevelData>(json);
            return levelData;
        }
        else
        {
            Debug.LogError("Level file not found: " + path);
            return null;
        }
    }

    public void LoadLevel(int level)
    {
        LevelData data = GetLevelTest(level.ToString());
        levelData = data;
    }

    public bool SetLevelPathCustom(string path)
    {
        levelPathCustom = path;
        if (Directory.Exists(levelPathCustom))
        {
            usePathCustom = !usePathCustom;

            return true;
        }
        else
        {
            usePathCustom = false;
            return false;
        }
    }

    public bool ExistLevel(int level)
    {
        string filePath = LEVEL_FILE_PATH + level;

        TextAsset textAsset = Resources.Load<TextAsset>(filePath);

        if (textAsset == null)
        {
            return false;
        }

        return true;
    }

    public void RestorePack(ShopItemKey key)
    {
        playerData.restoredPacks.Add((int)key);
        Save();
    }

    public void BuyItem()
    {
        playerData.buyCount++;
        Save();
    }

    public bool IsFirstPlay()
    {
        return playerData.saveLevelData.playCount == 1;
    }

    public void EnableNoAds()
    {
        //Kernel.Resolve<AdsManager>().EnableNoAds();
        //Kernel.Resolve<AdsManager>().DestroyBanner();

        isNoAds.Value = true;
    }

    public void DisableNoAds()
    {
        //Kernel.Resolve<AdsManager>().DisableNoAds();

        isNoAds.Value = false;
    }
}

[Serializable]
public class PlayerData
{
    public int preLevel = 0;
    public Level saveLevelData = new();
    public int nextLevel = 1;
    public int realLevel = 1;

    public string timeFirstOpen;

    public int coins;

    public List<Booster> boosters = new List<Booster>();
    public List<int> restoredPacks = new List<int>();

    public int numShowAds = 0;
    public int numCompleteRewardAds = 0;

    public int buyCount = 0;
}

[Serializable]
public class Level
{
    public int currentLevel;
    public int playCount;
}

[Serializable]
public class Booster
{
    public BoosterType type;
    public int amount;
    public string unlimitedTimeEnd;

    public Booster(BoosterType type, int amount, string unlimitedTimeEnd)
    {
        this.type = type;
        this.amount = amount;
        this.unlimitedTimeEnd = unlimitedTimeEnd;
    }
}

[Serializable]
public class LevelData
{
    public Difficulty difficulty;

    public List<CarData> carDatas = new();

    public List<GuestData> guestDatas = new();

    public List<GarageInfo> garageDatas = new();

    public float scaleFactor;
}

[Serializable]
public class GuestData
{
    public ObjectColor eColor;
    public int number;
}

[Serializable]
public class CarData
{
    public ObjectColor eColor;
    public Direction direction;
    public CarSize size;
    public Vector2Serialized position;
}

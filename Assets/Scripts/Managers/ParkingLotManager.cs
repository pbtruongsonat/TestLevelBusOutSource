using DarkTonic.PoolBoss;
using DG.Tweening;
using Obvious.Soap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingLotManager : SingletonBase<ParkingLotManager>
{
    [SerializeField] private Transform carPrefab;
    [SerializeField] private Transform garageTopPrefab;
    [SerializeField] private Transform garageTopLeftPrefab;
    [SerializeField] private Transform garageTopRightPrefab;
    [SerializeField] private Transform garageLeftPrefab;
    [SerializeField] private Transform garageRightPrefab;
    [SerializeField] private CarSO carSO;

    [SerializeField] private IntVariable coin;
    [SerializeField] private IntVariable currentLevel;
    [SerializeField] private BoolVariable isSelectingVipCar;
    [SerializeField] private BoolVariable isUsingMagnet;
    [SerializeField] private BoolVariable isBlockInteract;
    [SerializeField] private ScriptableEventNoParam OnStartLevel;

    public ParkingSpace vipParkingSpace;
    public List<ParkingSpace> parkingSpaces;
    public List<Car> carsInParking = new();
    public Transform parkingEntrance;
    public Transform carContainer;

    public List<Car> carsInGame = new();
    private List<Garage> garages = new();

    private List<ObjectColor> car4 = new(), car6 = new(), car10 = new();

    private int randomColorCount;

    private void Start()
    {
        OnStartLevel.OnRaised += Setup;

        Car.OnMoveAway += Car_OnMoveAway;
        Car.OnParking += Car_OnParking;

        isSelectingVipCar.OnValueChanged += OnUseParkingVip;
        isUsingMagnet.OnValueChanged += OnUseMagnet;
    }

    private void Setup()
    {
        DespawnAll();

        carsInParking.Clear();
        carsInGame.Clear();
        garages.Clear();

        InitParkingSpace();
        InitCars();
        InitGarages();
    }

    private void InitCars()
    {
        LevelData data = DataManager.Instance.levelData;

        carContainer.localScale = data.scaleFactor > 0 ? Vector3.one * data.scaleFactor : Vector3.one;

        foreach (CarData carData in data.carDatas)
        {
            Car car = PoolBoss.Spawn(carPrefab, Vector3.zero, Quaternion.identity, carContainer).GetComponent<Car>();
            car.transform.position = new Vector3(carData.position.x, carData.position.y, carData.position.y);
            car.transform.localScale = Vector3.one;
            car.Init(carData);

            carsInGame.Add(car);
        }
    }

    private void InitParkingSpace()
    {
        int amount = 4;

        vipParkingSpace.gameObject.SetActive(false);
        vipParkingSpace.Lock();

        for (int i = 0; i < parkingSpaces.Count; i++)
        {
            if (i <= amount && parkingSpaces[i] != vipParkingSpace)
            {
                parkingSpaces[i].Unlock();
            }
            else
            {
                parkingSpaces[i].Lock();
            }

            parkingSpaces[i].ActiveSortingGroup(false);
            parkingSpaces[i].StopWarning();
        }
    }

    private void InitGarages()
    {
        LevelData data = DataManager.Instance.levelData;

        if (data.garageDatas.Count == 0) return;

        foreach (var info in data.garageDatas)
        {
            Transform prefab;

            switch (info.direction)
            {
                case Direction.Left:
                    prefab = garageLeftPrefab; 
                    break;
                case Direction.Right:
                    prefab = garageRightPrefab;
                    break;
                case Direction.Top:
                    prefab = garageTopPrefab;
                    break;
                case Direction.TopLeft:
                    prefab = garageTopLeftPrefab;
                    break;
                case Direction.TopRight:
                    prefab = garageTopRightPrefab;
                    break;
                default:
                    return;
            }

            Garage garage = PoolBoss.Spawn(prefab, Vector3.zero, prefab.localRotation, carContainer).GetComponent<Garage>();
            garage.transform.position = new Vector3(info.position.x, info.position.y, info.position.y);
            garage.transform.localScale = Vector3.one;

            garage.Init(info);

            garages.Add(garage);
        }

        DOVirtual.DelayedCall(0.2f, () =>
        {
            foreach (Garage garage in garages)
            {
                garage.CheckMoveCar();
            }
        });

        
    }

    private void Car_OnParking(object sender, EventArgs e)
    {
        int emptySpaceLeftCount = GetEmptySpaceLeft();

        if (emptySpaceLeftCount == 1)
        {
            GetEmptyParkingSpace().Warning();
        }
        else if (emptySpaceLeftCount == 0) 
        {
            StopWarningAll();
        }
    }

    private void Car_OnMoveAway(object sender, EventArgs e)
    {
        Car car = sender as Car;

        carsInParking.Remove(car);
        carsInGame.Remove(car);

        coin.Value += (int)(car.Slot * GameDefine.COIN_PER_GUEST);
        //SonatTracking.LogEarnCurrency("coin", "currency", (int)(car.Slot * GameDefine.COIN_PER_GUEST), "ingame", "ingame", "non_iap",
        //    currentLevel.Value);

        int emptySpaceLeftCount = GetEmptySpaceLeft();

        if (emptySpaceLeftCount > 1)
        {
            StopWarningAll();
        }
        else if (emptySpaceLeftCount == 1)
        {
            GetEmptyParkingSpace().Warning();
        }

        foreach (Garage garage in garages)
        {
            if (garage.GarageInfo.cars.Count > 0) 
                return;
        }

        if (carsInGame.Count == 0)
        {
            car.isLastCar = true;
#if use_winlose
            GameplayManager.Instance.Win();
#endif
        }
    }

    public ParkingSpace GetEmptyParkingSpace()
    {
        foreach (var parkingSpace in parkingSpaces)
        {
            if (parkingSpace.CurrentStatus == ParkingSpaceStatus.Empty)
            {
                return parkingSpace;
            }
        }

        return null;
    }

    private void OnUseParkingVip(bool isUse)
    {
        if (isUse)
        {
            vipParkingSpace.UnlockVip();
            vipParkingSpace.PlayIdleEffect(true);
        }

        vipParkingSpace.ActiveSortingGroup(isUse);

        foreach (Car car in carsInGame)
        {
            if (isUse)
            {
                if (carsInParking.Contains(car) || car.IsMoving) continue;
            }

            car.ActiveSortingGroup(isUse);
        }
    }

    private void OnUseMagnet(bool isUse)
    {
        if (isUse)
        {
            GetEmptyParkingSpace().ActiveSortingGroup(true);
        }
        else
        {
            foreach (ParkingSpace parkingSpace in parkingSpaces)
            {
                parkingSpace.ActiveSortingGroup(false);
            }
        }

        foreach (Car car in carsInGame)
        {
            if (isUse)
            {
                if (carsInParking.Contains(car) || car.IsMoving) continue;
            }

            car.ActiveSortingGroup(isUse);
        }
    }

    public bool IsFullSpace()
    {
        int count = 0;

        foreach (ParkingSpace parkingSpace in parkingSpaces)
        {
            if (parkingSpace.CurrentStatus == ParkingSpaceStatus.Empty)
            {
                return false;
            }
            else if (parkingSpace.CurrentStatus == ParkingSpaceStatus.Busy)
            {
                count++;
            }
        }

        if (count == carsInParking.Count)
            return true;
        else
            return false;
    }

    public bool HasCarFull()
    {
        foreach (Car car in carsInParking)
        {
            if (car.IsFull()) return true;
        }

        return false;
    }

    public bool IsMaxSpace()
    {
        foreach (ParkingSpace parkingSpace in parkingSpaces)
        {
            if (parkingSpace.CurrentStatus == ParkingSpaceStatus.Lock && parkingSpace != vipParkingSpace)
                return false;
        }

        return true;
    }

    private int GetEmptySpaceLeft()
    {
        int count = 0;

        foreach (ParkingSpace parkingSpace in parkingSpaces)
        {
            if (parkingSpace.CurrentStatus == ParkingSpaceStatus.Empty)
            {
                count++;
            }
        }

        return count;
    }

    public void UnlockNextSpace()
    {
        foreach (ParkingSpace parkingSpace in parkingSpaces)
        {
            if (parkingSpace.CurrentStatus == ParkingSpaceStatus.Lock && parkingSpace != vipParkingSpace)
            {
                parkingSpace.Unlock();
                parkingSpace.Warning();
                break;
            }
        }

        //SonatTracking.LogEarnCurrency("add_parking_lot", "booster", 1, "lose", "out_of_space", "non_iap", currentLevel.Value);
        //SonatTracking.LogUseBooster(currentLevel.Value, "add_parking_lot");
        //SonatTracking.LogSpendCurrency("add_parking_lot", "booster", 1, "lose", "out_of_space");
    }

    public void DespawnAll()
    {
        foreach (Car car in carsInGame)
        {
            car.transform.DOKill();

            car.DespawnGuestInCar();
            car.Destroy();
        }

        foreach (Garage garage in garages)
        {
            garage.Destroy();
        }
    }

    public void RandomCarColor()
    {
        randomColorCount = 0;

        isBlockInteract.Value = true;

        SaveOriginColor();

        StartCoroutine(IeRandomCarColor());
    }

    public IEnumerator IeRandomCarColor()
    {
        Array values = Enum.GetValues(typeof(ObjectColor));
        System.Random random = new System.Random();

        foreach (Car car in carsInGame)
        {
            if (carsInParking.Contains(car) || car.IsMoving) continue;

            ObjectColor randomColor = (ObjectColor)values.GetValue(random.Next(values.Length));

            if (randomColor == ObjectColor.None) continue;

            car.ChangeColor(randomColor);
        }

        randomColorCount++;

        yield return new WaitForSeconds(0.35f);

        if (randomColorCount < 3)
        {
            StartCoroutine(IeRandomCarColor());
        }
        else
        {
            ArrangeColorAfterShuffle();

            isBlockInteract.Value = false;
        }
    }

    private void SaveOriginColor()
    {
        car4.Clear();
        car6.Clear();
        car10.Clear();

        foreach (Car car in carsInGame)
        {
            if (carsInParking.Contains(car) || car.IsMoving) continue;

            if (car.Size == CarSize.Small4Slots)
            {
                car4.Add(car.Color);
            }
            else if (car.Size == CarSize.Medium6Slots)
            {
                car6.Add(car.Color);
            }
            else if (car.Size == CarSize.Big10Slots)
            {
                car10.Add(car.Color);
            }
        }
    }

    private void ArrangeColorAfterShuffle()
    {
        foreach (Car car in carsInGame)
        {
            if (carsInParking.Contains(car) || car.IsMoving) continue;

            if (car.Size == CarSize.Small4Slots)
            {
                ObjectColor randomColor = car4.GetRandomElementInList();
                car.ChangeColor(randomColor);
                car4.Remove(randomColor);
            }
            else if (car.Size == CarSize.Medium6Slots)
            {
                ObjectColor randomColor = car6.GetRandomElementInList();
                car.ChangeColor(randomColor);
                car6.Remove(randomColor);
            }
            else if (car.Size == CarSize.Big10Slots)
            {
                ObjectColor randomColor = car10.GetRandomElementInList();
                car.ChangeColor(randomColor);
                car10.Remove(randomColor);
            }
        }
    }

    public bool HasPossibleClickCar()
    {
        bool result = false;

        foreach (Car car in carsInGame)
        {
            if (carsInParking.Contains(car) || car.IsMoving) continue;

            result = true;
        }

        return result;
    }

    public Car GetTutCar(int index)
    {
        LevelData data = DataManager.Instance.levelData;

        if (index >= data.guestDatas.Count) return null;

        foreach (Car car in carsInGame)
        {
            if (carsInParking.Contains(car) || car.IsMoving) continue;

            if (!car.CanMove()) continue;

            if (car.Color == data.guestDatas[index].eColor) return car;
        }

        return null;
    }

    public void StopWarningAll()
    {
        foreach (var parkingSpace in parkingSpaces)
        {
            parkingSpace.StopWarning();
        }
    }
}

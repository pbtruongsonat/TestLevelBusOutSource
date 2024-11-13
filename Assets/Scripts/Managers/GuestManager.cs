using DarkTonic.PoolBoss;
using DG.Tweening;
using Obvious.Soap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GuestManager : SingletonBase<GuestManager>
{
    [SerializeField] private Transform guestPrefab;
    [SerializeField] private List<Transform> waitingPosition;
    [SerializeField] private Transform waitEntrance;
    [SerializeField] private Transform guestContainer;
    [SerializeField] private TextMeshPro guestCount;

    [SerializeField] private BoolVariable isBlockInteract;
    [SerializeField] private ScriptableEventNoParam OnStartLevel;
    [SerializeField] private ScriptableEventNoParam OnCheckTutorial;

    [SerializeField] private List<Guest> guestInLine = new();
    [SerializeField] private List<Guest> guestList = new();

    [SerializeField] private float minEmojiTime;
    [SerializeField] private float maxEmojiTime;
    [SerializeField] private int delayEmojiHappyCount;

    private float emojiTimer;
    private bool isDelayFirstEmoji;
    [SerializeField] private int emojiHappyCount;

    private void Start()
    {
        OnStartLevel.OnRaised += Setup;

        Car.OnParking += Car_OnParking;
        Guest.OnReadyToEnterCar += Guest_OnReadyToEnterCar;
    }

    private void Setup()
    {
        StopAllCoroutines();

        DespawnAll();

        isBlockInteract.Value = true;

        emojiTimer = 0;

        guestInLine.Clear();
        guestList.Clear();

        LevelData data = DataManager.Instance.levelData;

        foreach (GuestData guestData in data.guestDatas)
        {
            for (int i = 0; i < guestData.number; i++)
            {
                Guest spawned = PoolBoss.Spawn(guestPrefab, waitEntrance.position, Quaternion.identity, guestContainer).GetComponent<Guest>();

                spawned.transform.SetAsFirstSibling();

                spawned.Init(guestData.eColor);

                guestInLine.Add(spawned);
                guestList.Add(spawned);
            }
        }

        StartCoroutine(IeStartMoveOut());

        guestCount.text = guestInLine.Count.ToString();
    }

    private IEnumerator IeStartMoveOut()
    {
        List<Vector3> path = new List<Vector3>();

        int count = 0;

        for (int i = 0; i < guestInLine.Count; i++)
        {
            yield return new WaitForSeconds(0.1f);

            path.Clear();

            for (int j = waitingPosition.Count - 1; j >= count; j--)
            {
                path.Add(waitingPosition[j].position);
            }

            if (path.Count == 0) break;

            guestInLine[i].MoveTo(path.ToArray());

            count++;
        }

        isBlockInteract.Value = false;

        OnCheckTutorial.Raise();

        isDelayFirstEmoji = true;
        emojiHappyCount = 0;

        StartCoroutine(nameof(IeShowEmoji));
    }

    private IEnumerator IeShowEmoji()
    {
        if (isDelayFirstEmoji)
        {
            isDelayFirstEmoji = false;
            yield return new WaitForSeconds(maxEmojiTime);
        }

        float randomTime = Random.Range(minEmojiTime, maxEmojiTime);

        while (emojiTimer < randomTime)
        {
            emojiTimer += Time.deltaTime;

            yield return null;
        }

        if (guestList.Count == 0) yield break;

        int guestIndex = Random.Range(0, Mathf.Min(guestList.Count, 11));

        if (guestList[guestIndex].IsInCar)
        {
            if (emojiHappyCount < delayEmojiHappyCount)
            {
                emojiHappyCount++;
            }
            else
            {
                guestList[guestIndex].ShowEmoji();
                emojiHappyCount = 0;
            }
        }
        else
        {
            guestList[guestIndex].ShowEmoji();
        }

        emojiTimer = 0;

        StartCoroutine(nameof(IeShowEmoji));
    }

    private void Car_OnParking(object sender, System.EventArgs e)
    {
        Guest guest = GetFirstGuestInLine();

        if (guest == null) return;

        CheckMoveToCar(guest, GetReadyCar(guest.Color));
    }

    private void CheckMoveToCar(Guest guest, Car car)
    {
        if (car == null || car.IsFull()) return;

        int index = 0;

        foreach (Guest guestInLine in guestInLine)
        {
            if (guestInLine == guest) continue;

            guestInLine.MoveTo(waitingPosition[index].position);

            if (index < waitingPosition.Count - 1)
            {
                index++;
            }

            if (index >= 20) break;
        }

        guest.MoveToCar(car);

        guestInLine.Remove(guest);

        guestCount.text = guestInLine.Count.ToString();

        emojiTimer = 0;
        isDelayFirstEmoji = true;
    }

    private void Guest_OnReadyToEnterCar(object sender, System.EventArgs e)
    {
        Guest firstGuest = GetFirstGuestInLine();
        if (firstGuest == null) return;

        Guest guest = sender as Guest;
        if (guest != firstGuest) return;

        if (guest.IsMovingInLine) return;

        Car readyCar = GetReadyCar(guest.Color);
        if (readyCar == null)
        {
            return;
        }

        CheckMoveToCar(guest, readyCar);
    }

    public Guest GetFirstGuestInLine()
    {
        if (guestInLine.Count == 0)
            return null;
        else
            return guestInLine[0];
    }

    private Car GetReadyCar(ObjectColor color)
    {
        List<Car> readyCars = new();

        foreach (Car car in ParkingLotManager.Instance.carsInParking)
        {
            if (car.IsFull()) continue;

            if (car.Color == color)
                readyCars.Add(car);
        }

        if (readyCars.Count == 0)
        {
            CheckLose();
            return null;
        }

        readyCars = readyCars.OrderByDescending(c => c.guests.Count).ToList();

        return readyCars[0];
    }

    public bool IsEdgeGuest(Guest guest)
    {
        return guestInLine.IndexOf(guest) == 11;
    }

    public void RemoveGuest(Guest guest)
    {
        guestList.Remove(guest);
    }

    public void DespawnAll()
    {
        foreach (Guest guest in guestList)
        {
            guest.transform.DOKill();
            PoolBoss.Despawn(guest.transform);
        }
    }

    private void CheckLose()
    {
        Debug.Log("abc check lose");

        if (ParkingLotManager.Instance.IsFullSpace() && !ParkingLotManager.Instance.HasCarFull())
        {
#if use_winlose
            GameplayManager.Instance.Lose();
#endif
        }
    }

    public void SortingGuest()
    {
        List<ObjectColor> originalColors = guestInLine.Select(guest => guest.Color).ToList();
        List<ObjectColor> sortedColors = new List<ObjectColor>();
        Dictionary<ObjectColor, int> colorsNeedSort = new();
        // car color in parking lot
        foreach (Car car in ParkingLotManager.Instance.carsInParking)
        {
            int slotToFill = car.Slot - car.guests.Count;

            if (slotToFill > 0)
            {
                if (colorsNeedSort.ContainsKey(car.Color))
                {
                    colorsNeedSort[car.Color] += slotToFill;
                }
                else
                {
                    colorsNeedSort.Add(car.Color, slotToFill);
                }
            }
        }

        int count = 0;

        Dictionary<ObjectColor, int> colorsNeedSort1 = new();
        // car outside
        foreach (Car car in ParkingLotManager.Instance.carsInGame)
        {
            if (ParkingLotManager.Instance.carsInParking.Contains(car) || car.IsMoving) continue;

            if (!car.CanMove()) continue;

            if (colorsNeedSort1.ContainsKey(car.Color))
            {
                colorsNeedSort1[car.Color] += car.Slot;
            }
            else
            {
                colorsNeedSort1.Add(car.Color, car.Slot);
            }

            count++;

            if (count >= 5) break;
        }

        foreach (var colorNeedSort in colorsNeedSort)
        {
            int colorCount = 0;

            for (int i = 0; i < originalColors.Count; i++)
            {
                if (colorCount == colorNeedSort.Value) break;

                if (originalColors[i] == colorNeedSort.Key && colorCount < colorNeedSort.Value)
                {
                    sortedColors.Add(originalColors[i]);
                    colorCount++;

                    originalColors.RemoveAt(i);
                    i--;
                }
            }
        }

        foreach (var colorNeedSort in colorsNeedSort1)
        {
            int colorCount = 0;

            for (int i = 0; i < originalColors.Count; i++)
            {
                if (colorCount == colorNeedSort.Value) break;

                if (originalColors[i] == colorNeedSort.Key && colorCount < colorNeedSort.Value)
                {
                    sortedColors.Add(originalColors[i]);
                    colorCount++;

                    originalColors.RemoveAt(i);
                    i--;
                }
            }
        }
        // add remain color
        sortedColors.AddRange(originalColors);

        for (int i = 0; i < sortedColors.Count; i++)
        {
            guestInLine[i].ChangeColor(sortedColors[i]);
        }

        Guest guest = GetFirstGuestInLine();

        if (guest == null) return;

        CheckMoveToCar(guest, GetReadyCar(guest.Color));
    }
}

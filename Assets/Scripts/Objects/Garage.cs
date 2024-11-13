using DarkTonic.PoolBoss;
using Obvious.Soap;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Garage : MonoBehaviour
{
    [SerializeField] private GarageInfo garageInfo;
    [SerializeField] private PolygonCollider2D entrance4Slot;
    [SerializeField] private PolygonCollider2D entrance10Slot;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SpriteRenderer garageDisplay;
    [SerializeField] private SpriteRenderer carLight;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private TextMeshPro carCount;
    [SerializeField] private Transform carPrefab;

    [SerializeField] private CarSO carSO;

    [SerializeField] private BoolVariable isSelectingVipCar;
    [SerializeField] private BoolVariable isUsingMagnet;

    public GarageInfo GarageInfo => garageInfo;

    public void Init(GarageInfo info)
    {
        garageInfo = info;
        carCount.text = info.cars.Count.ToString();

        UpdateLight();
    }

    private void OnEnable()
    {
        Car.OnMoveToParkingSpace += Car_OnMoveToParkingSpace;

        isSelectingVipCar.OnValueChanged += IsSelectingVipCar_OnValueChanged;
        isUsingMagnet.OnValueChanged += IsUsingMagnet_OnValueChanged;
    }

    private void OnDisable()
    {
        Car.OnMoveToParkingSpace -= Car_OnMoveToParkingSpace;

        isSelectingVipCar.OnValueChanged -= IsSelectingVipCar_OnValueChanged;
        isUsingMagnet.OnValueChanged -= IsUsingMagnet_OnValueChanged;
    }

    private void Car_OnMoveToParkingSpace(object sender, EventArgs e)
    {
        if (garageInfo.cars.Count == 0) return;

        CheckMoveCar();
    }

    private void IsSelectingVipCar_OnValueChanged(bool value)
    {
        if (garageInfo.cars.Count == 0) return;

        if (!value)
        {
            if (isSelectingVipCar.PreviousValue)
            {
                CheckMoveCar();
            }
        }
    }

    private void IsUsingMagnet_OnValueChanged(bool value)
    {
        if (garageInfo.cars.Count == 0) return;

        if (!value)
        {
            if (isUsingMagnet.PreviousValue)
            {
                CheckMoveCar();
            }
        }
    }

    public void CheckMoveCar()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(layerMask);
        contactFilter.useTriggers = true;

        List<Collider2D> overlapCollider = new List<Collider2D>();

        int overlapCount;

        if (garageInfo.cars[0].size == CarSize.Big10Slots)
        {
            overlapCount = entrance10Slot.OverlapCollider(contactFilter, overlapCollider);
        }
        else
        {
            overlapCount = entrance4Slot.OverlapCollider(contactFilter, overlapCollider);
        }

        if (overlapCount > 0)
        {
            //foreach (Collider2D col in overlapCollider)
            //{
            //    Debug.Log("abc " + col.gameObject);
            //}
        }
        else
        {
            MoveCar();
        }
    }

    private void MoveCar()
    {
        Car firstCar = PoolBoss.Spawn(carPrefab, spawnPoint.position, Quaternion.identity, ParkingLotManager.Instance.carContainer).GetComponent<Car>();
        firstCar.transform.localScale = Vector3.zero;

        ParkingLotManager.Instance.carsInGame.Add(firstCar);

        firstCar.Init(garageInfo.cars[0], false);

        firstCar.MoveOutOfGarage(firstCar.Size == CarSize.Big10Slots ? entrance10Slot.transform.position : entrance4Slot.transform.position);

        garageInfo.cars.RemoveAt(0);
        carCount.text = garageInfo.cars.Count.ToString();

        UpdateLight();
    }

    private void UpdateLight()
    {
        if (garageInfo.cars.Count == 0)
        {
            carLight.gameObject.SetActive(false);
        }
        else
        {
            Sprite light = carSO.GetGarageLightSprite(garageInfo.cars[0].eColor);

            if (light != null)
            {
                carLight.gameObject.SetActive(true);

                carLight.sprite = light;
            }
        }
    }

    public void Destroy()
    {
        PoolBoss.Despawn(transform);
    }
}

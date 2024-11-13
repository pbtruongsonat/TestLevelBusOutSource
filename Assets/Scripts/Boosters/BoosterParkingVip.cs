using Obvious.Soap;
using UnityEngine;

public class BoosterParkingVip : BoosterBase
{
    [SerializeField] private BoolVariable isSelectingVipCar;

    public override void Use()
    {
        base.Use();

        isSelectingVipCar.Value = true;

        gameObject.SetActive(false);
    }
}

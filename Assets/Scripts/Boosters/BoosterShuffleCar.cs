using Obvious.Soap;
using UnityEngine;

public class BoosterShuffleCar : BoosterBase
{
    public override void Use()
    {
        base.Use();

        ParkingLotManager.Instance.RandomCarColor();
        EffectManager.Instance.ShowShuffleCarEffect();
    }
}

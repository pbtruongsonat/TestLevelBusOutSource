using Obvious.Soap;
using UnityEngine;

public class BoosterMagnet : BoosterBase
{
    [SerializeField] private BoolVariable isUsingMagnet;

    public override void Use()
    {
        if (ParkingLotManager.Instance.GetEmptyParkingSpace() == null)
        {
            PopupManager.Instance.ShowNotiAlert("No empty space!");
            return;
        }

        base.Use();

        isUsingMagnet.Value = true;
    }
}

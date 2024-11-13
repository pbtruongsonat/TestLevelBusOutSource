using System.Collections.Generic;
using UnityEngine;

public class SaveSlotPos : MonoBehaviour
{
    [SerializeField] private List<Transform> slotPos;
    [SerializeField] private CarSize size;
    [SerializeField] private CarSO carSO;

    public void SavePos()
    {
        CarSizeConfig data = carSO.GetCarSlot(size);

        for (int i = 0; i < data.slotPositions.Count; i++)
        {
            data.slotPositions[i] = slotPos[i].position;           
        }
    }

    public void LoadPos()
    {
        CarSizeConfig data = carSO.GetCarSlot(size);

        for (int i = 0; i < data.slotPositions.Count; i++)
        {
            slotPos[i].position = data.slotPositions[i];
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterSortingGuest : BoosterBase
{
    public override void Use()
    {
        base.Use();

        GuestManager.Instance.SortingGuest();
    }
}

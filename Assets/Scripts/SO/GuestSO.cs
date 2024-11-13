using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/GuestSO", fileName = "GuestSO")]
public class GuestSO : ScriptableObject
{
    public List<GuestInfo> guestInfos;

   public RuntimeAnimatorController GetGuestAnimation(ObjectColor color)
    {
        var info = guestInfos.Find(i => i.color == color);

        if (info == null) return null;

        return info.guestAnimation;
    }
}

[Serializable]
public class GuestInfo
{
    public ObjectColor color;
    public RuntimeAnimatorController guestAnimation;
}


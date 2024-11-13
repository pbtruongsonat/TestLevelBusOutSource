using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupPolicy : PopupBase
{
    public override void OnPush(Data data)
    {
        base.OnPush(data);

        //SonatTracking.LogShowUI("user", currentLevel.Value, "policy", "ingame", "settings", "open");
    }

    public override void OnPop()
    {
        base.OnPop();

        //SonatTracking.LogShowUI("user", currentLevel.Value, "policy", "ingame", "settings", "close");
    }
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/BoosterSO", fileName = "BoosterSO")]
public class BoosterSO : ScriptableObject
{
    public List<BoosterInfo> boosters;

    public BoosterInfo GetBoosterInfo(BoosterType type)
    {
        return boosters.Find(b => b.type == type);
    }
}

[System.Serializable]
public class BoosterInfo
{
    public BoosterType type;
    public string name;
    public string description;
    public Sprite icon;
    public Sprite bigIcon;
    public int cost;
}
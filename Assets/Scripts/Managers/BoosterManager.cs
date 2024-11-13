using Obvious.Soap;
using System.Collections.Generic;
using UnityEngine;

public class BoosterManager : SingletonBase<BoosterManager>
{
    public List<BoosterBase> boosterBases = new();

    [SerializeField] private IntVariable currentLevel;
    [SerializeField] private ScriptableEventNoParam OnStartLevel;

    private void Start()
    {
        OnStartLevel.OnRaised += ResetData;
    }

    private void ResetData()
    {
        boosterBases.SetActiveAll(currentLevel.Value > 1);
        boosterBases.LoadData();
    }

    public void AddBooster(BoosterType type, int value, bool isLog = true)
    {
        foreach (var booster in boosterBases)
        {
            if (booster.Type == type)
            {
                booster.Add(value, isLog);
                return;
            }
        }
    }
}

using DarkTonic.PoolBoss;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonBase<EffectManager>
{
    [SerializeField] private List<Transform> emojiWait; 
    [SerializeField] private List<Transform> emojiSit;
    [SerializeField] private Transform coinEffect;
    [SerializeField] private Transform clickCarEffect;
    [SerializeField] private Transform shuffleCarEffect;
    [SerializeField] private Transform carDisappearEffect;
    [SerializeField] private Transform carAppearEffect;

    private void Start()
    {
        Car.OnMoveAway += Car_OnMoveAway;
    } 

    public void ShowEmoji(Vector3 position, bool isWaiting)
    {
        Transform random;

        if (isWaiting)
        {
            random = emojiWait.GetRandomElementInList();
        }
        else
        {
            random = emojiSit.GetRandomElementInList();
        }

        PoolBoss.Spawn(random, position, Quaternion.identity, null);
    }

    private void Car_OnMoveAway(object sender, System.EventArgs e)
    {
        PoolBoss.Spawn(coinEffect, (sender as Car).transform.position, Quaternion.identity, null);
    }

    public void ShowClickCarEffect(Vector3 position)
    {
        PoolBoss.Spawn(clickCarEffect, position, Quaternion.identity, null);
    }

    public void ShowShuffleCarEffect()
    {
        PoolBoss.Spawn(shuffleCarEffect, shuffleCarEffect.position, shuffleCarEffect.localRotation, null);
    }

    public void ShowCarDisappearEffect(Vector3 pos)
    {
        PoolBoss.Spawn(carDisappearEffect, pos, Quaternion.identity, null);
    }

    public void ShowCarAppearEffect(Vector3 pos)
    {
        PoolBoss.Spawn(carAppearEffect, pos, Quaternion.identity, null);
    }
}

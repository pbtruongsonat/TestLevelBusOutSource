using DarkTonic.PoolBoss;
using UnityEngine;

public class EffectDestroySelf : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        PoolBoss.Despawn(transform);
    }
}

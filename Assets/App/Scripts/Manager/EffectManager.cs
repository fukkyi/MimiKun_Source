using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : AutoGenerateManagerBase<EffectManager>
{
    [SerializeField]
    private ObjectPool[] particlePools = new ObjectPool[0];

    /// <summary>
    /// パーティクルを生成する
    /// </summary>
    /// <param name="particleType"></param>
    /// <param name="position"></param>
    public void PlayParticle(ParticleType particleType, Vector3 position)
    {
        int poolIndex = (int)particleType;

        if (poolIndex < 0 && poolIndex >= particlePools.Length) return;

        ObjectPool particlePool = particlePools[(int)particleType];
        ParticleObject particle = particlePool.GetObject<ParticleObject>();

        particle.PlayOfPosition(position);
    }
}

public enum ParticleType
{
    DroneExplosion = 0,
}
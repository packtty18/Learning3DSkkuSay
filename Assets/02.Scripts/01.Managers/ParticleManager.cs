using System.Collections.Generic;
using UnityEngine;

public enum EParticleType
{ 
    BombExplosion,
    BulletEnvironmentHit,
}


public class ParticleManager : Singleton<ParticleManager>
{
    [Header("Particle Data List")]
    [SerializeField] private ParticleSO[] _particleDatas;

    // SO 데이터 캐싱
    private readonly Dictionary<EParticleType, ParticleSO> _dataDict =
        new Dictionary<EParticleType, ParticleSO>();

    // 실제 생성된 파티클 저장
    private readonly Dictionary<EParticleType, ParticleSystem> _instanceDict =
        new Dictionary<EParticleType, ParticleSystem>();

    public override void Init()
    {
        foreach (var data in _particleDatas)
        {
            if (data == null) continue;

            if (!_dataDict.ContainsKey(data.type))
            {
                _dataDict.Add(data.type, data);
            }
        }
    }
    public ParticleSystem Get(EParticleType type)
    {
        ParticleSystem ps = GetOrCreateInstance(type);
        return ps;
    }

    private ParticleSystem GetOrCreateInstance(EParticleType type)
    {
        if (_instanceDict.TryGetValue(type, out ParticleSystem ps))
            return ps;

        if (!_dataDict.TryGetValue(type, out ParticleSO data))
        {
            DebugManager.Instance.LogWarning($"[ParticleManager] No FX Data found for {type}");
            return null;
        }

        GameObject instant = Instantiate(data.particlePrefab, transform);
        if(!instant.TryGetComponent(out ParticleSystem newPs))
        {
            return null;
        }

        newPs.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        _instanceDict[type] = newPs;
        DebugManager.Instance.Log($"[ParticleManager] Created FX Instance: {type}");

        return newPs;
    }

    
}

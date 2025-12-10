using System.Collections;
using UnityEngine;

public class ExplosionCallback : MonoBehaviour, IPoolable
{
    private ParticleSystem _particle;
    private Coroutine _autoReleaseRoutine;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    public void Get()
    {
        if (_autoReleaseRoutine != null)
            StopCoroutine(_autoReleaseRoutine);

        _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        _particle.Play();
        _autoReleaseRoutine = StartCoroutine(AutoReleaseRoutine());
    }

    public void Release()
    {
        if (_autoReleaseRoutine != null)
            StopCoroutine(_autoReleaseRoutine);

        _particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private IEnumerator AutoReleaseRoutine()
    {
        while (_particle.IsAlive(true))
        {
            yield return null;
        }

        PoolManager.Instance.Release(EPoolType.Explosion, gameObject);
    }
}

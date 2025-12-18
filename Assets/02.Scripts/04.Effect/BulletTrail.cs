using System.Collections;
using UnityEngine;

public class BulletTrail : MonoBehaviour, IPoolable
{
    private EPoolType _poolType = EPoolType.BulletTrail;
    public EPoolType PoolType => _poolType;

    [Header("Settings")]
    [SerializeField] private float _delay = 0.1f;
    [SerializeField] private float _speed = 60f;

    private Coroutine _moveRoutine;

    public void Get(EPoolType type)
    {
        if (_poolType == EPoolType.None)
            _poolType = type;

        gameObject.SetActive(true);
    }

    public void Release()
    {
        if (_moveRoutine != null)
        {
            StopCoroutine(_moveRoutine);
            _moveRoutine = null;
        }

    }

    public void SetDestination(Vector3 position)
    {
        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);

        _moveRoutine = StartCoroutine(MoveTo(position));
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.SqrMagnitude(transform.position - target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                _speed * Time.deltaTime
            );
            yield return null;
        }

        yield return new WaitForSeconds(_delay);

        Util.DestroyImmediate(gameObject);
    }
}

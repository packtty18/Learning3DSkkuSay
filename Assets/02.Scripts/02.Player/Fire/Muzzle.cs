using UnityEngine;

public class Muzzle : MonoBehaviour, IPoolable
{
    private EPoolType _pooltype = EPoolType.Muzzle;
    public EPoolType PoolType => _pooltype;

    [SerializeField] private Material[] _material;
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private float _activeTime = 0.05f;

    private Coroutine _coroutine;
    public void Get(EPoolType type)
    {
        int random = Random.Range(0, _material.Length);
        _mesh.material = _material[random];

        _coroutine = StartCoroutine(Util.DestroyAfterTime(_activeTime,gameObject));
    }

    public void Release()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    
}

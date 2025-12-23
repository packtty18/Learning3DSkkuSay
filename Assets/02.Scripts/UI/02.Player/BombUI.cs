using UnityEngine;
using UnityEngine.UI;

public class BombUI : MonoBehaviour
{
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private Image[] _bombIcons;

    private IReadOnlyConsumable<int> _bombCount => _stat.BombCount;
    private void Start()
    {
        SetCount(_stat.BombCount.Current);
    }

    private void OnEnable()
    {
        _stat.BombCount.Subscribe(SetCount);
    }

    private void OnDisable()
    {
        _stat.BombCount.Unsubscribe(SetCount);
    }

    private void SetCount(int current)
    {
        for (int i = 0; i < _bombIcons.Length; i++)
        {
            _bombIcons[i].gameObject.SetActive(i < current);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class BombUI : MonoBehaviour
{
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private Image[] _bombIcons;

    private void OnEnable()
    {
        if (_stat == null || _stat.BombCount == null)
            return;

        _stat.BombCount.OnCurrentChanged += SetCount;

        SetCount(_stat.BombCount.Current);
    }

    private void OnDisable()
    {
        if (_stat == null || _stat.BombCount == null)
            return;

        _stat.BombCount.OnCurrentChanged -= SetCount;
    }

    private void SetCount(int current)
    {
        for (int i = 0; i < _bombIcons.Length; i++)
        {
            _bombIcons[i].gameObject.SetActive(i < current);
        }
    }
}

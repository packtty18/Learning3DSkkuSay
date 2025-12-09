using UnityEngine;
using UnityEngine.UI;

public class BombUI : MonoBehaviour
{
    [SerializeField] private PlayerStat _stat;
    [SerializeField] private Image[] _bombIcons;

    private float currentCount => _stat.BombCount.CurrentValue;

    private void OnEnable()
    {
        _stat.BombCount.ValueChanged += SetCount;
    }

    private void OnDisable()
    {
        _stat.BombCount.ValueChanged -= SetCount;
    }

    public void SetCount()
    {
        for (int i = 0; i < _bombIcons.Length; i++)
        {
            bool isActive = i < currentCount;
            _bombIcons[i].gameObject.SetActive(isActive);
        }
    }
}

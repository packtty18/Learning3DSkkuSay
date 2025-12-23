using ArtificeToolkit.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class BombUI : MonoBehaviour
{
    [Title("Player Reference")]
    [Required, SerializeField]
    private PlayerStat _stat;

    [Title("UI")]
    [Required, SerializeField]
    private Image[] _bombIcons;
    private IReadOnlyConsumable<int> _bombCount => _stat.GetConsumable(EConsumableInt.BombCount);

    private void OnEnable()
    {
        if(_bombCount == null)
        {
            _stat.OnStatInitEnd.Subscribe(UIEnable);
            return;
        }

        UIEnable();
    }

    public void UIEnable()
    {
        _bombCount.Subscribe(SetCount);
        SetCount(_bombCount.Current);
    }

    private void OnDisable()
    {
        _bombCount.Unsubscribe(SetCount);
    }

    private void SetCount(int current)
    {
        for (int i = 0; i < _bombIcons.Length; i++)
        {
            _bombIcons[i].gameObject.SetActive(i < current);
        }
    }
}

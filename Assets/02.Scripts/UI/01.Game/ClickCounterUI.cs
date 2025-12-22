using TMPro;
using UnityEngine;

public class ClickCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _leftText;
    [SerializeField] private TextMeshProUGUI _rightText;


    private void OnEnable()
    {
        ClickManager.Instance.OnLeftClick += CountLeft;
        ClickManager.Instance.OnRightClick += CountRight;
    }

    private void OnDisable()
    {
        ClickManager.Instance.OnLeftClick -= CountLeft;
        ClickManager.Instance.OnRightClick -= CountRight;
    }

    private void Start()
    {
        CountLeft(0);
        CountRight(0);
    }

    private void CountLeft(int count)
    {
        _leftText.text = $"Left Click : {count}";
    }

    private void CountRight(int count)
    {
        _rightText.text = $"Right Click : {count}";
    }
}

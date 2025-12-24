using TMPro;
using UnityEngine;

public class ClickCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _leftText;
    [SerializeField] private TextMeshProUGUI _rightText;


    private void OnEnable()
    {
        Subscribing();
    }

    private void Subscribing()
    {
        if(!ClickManager.IsExist())
        {
            return;
        }

        ClickManager.Instance.OnLeftClick.Subscribe(CountLeft);
        ClickManager.Instance.OnRightClick.Subscribe(CountRight);
    }

    private void OnDisable()
    {
        UnsubScribing();
    }

    private void UnsubScribing()
    {
        ClickManager.Instance.OnLeftClick.Unsubscribe(CountLeft);
        ClickManager.Instance.OnRightClick.Unsubscribe(CountRight);
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

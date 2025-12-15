using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbarUI : MonoBehaviour
{
    [SerializeField] private EnemyHealth _enemy;
    [SerializeField] private Image _frontGuage;
    [SerializeField] private Image _backGuage;
    private void OnEnable()
    {
        _enemy.OnHealthChange += UpdateUI;
    }

    private void OnDisable()
    {
        _enemy.OnHealthChange -= UpdateUI;
    }

    private void UpdateUI()
    {
        _frontGuage.fillAmount = _enemy.Current / _enemy.MaxHealth;
    }
}

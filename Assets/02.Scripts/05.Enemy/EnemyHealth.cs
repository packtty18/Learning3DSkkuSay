using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 100f;
    private float _current;
    public float Current => _current;

    private void Awake()
    {
        _current = MaxHealth;
    }


    public bool ApplyDamage(float damage)
    {
        _current -= damage;
        
        return _current > 0;
    }
}

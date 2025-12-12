using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float current;

    private void Awake()
    {
        current = maxHealth;
    }


    public bool ApplyDamage(float damage)
    {
        current -= damage;
        DebugManager.Instance.Log($"피격 발생 : {gameObject.name}에게 {damage} 데미지. 남은체력 : {current} / {maxHealth}");
        return current > 0;
    }
}

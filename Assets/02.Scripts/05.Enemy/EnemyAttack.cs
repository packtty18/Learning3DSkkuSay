using UnityEngine;

public class EnemyAttack : MonoBehaviour, IAttackable
{
    [SerializeField]private EnemyState _state;
    private PlayerHealth _player => _state.Player.GetComponent<PlayerHealth>();
    public void Attack(AttackData data)
    {
        Debug.Log("적 공격 발동!");
        _player.ApplyDamage(data);
    }
}

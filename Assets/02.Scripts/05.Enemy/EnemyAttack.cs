using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]private EnemyController _controller;
    private PlayerHealth _player => _controller.Player.GetComponent<PlayerHealth>();
    public void Attack()
    {
        float distance = Vector3.Distance(_controller.transform.position, _player.transform.position);
        if (distance > _controller.Stat.AttackDistance.Value)
        {
            return;
        }

        AttackData data = new AttackData
        {
            Damage = _controller.Stat.AttackDamage.Value,
            HitDirection = _controller.transform.forward,
            Attacker = _controller.gameObject
        };

        DebugManager.Instance.Log("적 공격 발동!");
        _player.ApplyDamage(data);
    }
}

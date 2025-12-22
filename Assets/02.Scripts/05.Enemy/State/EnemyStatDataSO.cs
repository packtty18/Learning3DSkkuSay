using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/StateConfig")]
public class EnemyStatDataSO :ScriptableObject 
{
    public float Health;
    public float MoveSpeed;
    public float AttackDamage;
    public float AttackSpeed;
    public float AttackDistance;
    public float DetectDistance;
}

using UnityEngine;

public interface IAttackable
{
    void Attack(AttackData data);
}

public struct AttackData
{
    public float Damage; //데미지
    public Vector3 HitDirection; //공격 방향 
    public GameObject Attacker;  //공격의 주체

    public AttackData(float damage, Vector3 hitDirection, GameObject attacker)
    {
        Damage = damage;
        HitDirection = hitDirection;
        Attacker = attacker;
    }
}
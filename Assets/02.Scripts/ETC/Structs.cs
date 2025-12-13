
using System;
using UnityEngine;

[Serializable]
public struct RecoilData
{
    //반동
    public float RecoilUp;
    public float RecoilSide;

    public RecoilData(float up, float side)
    {
        RecoilUp = up;
        RecoilSide = side;
    }
}

[Serializable]
public struct KnockbackData
{
    //넉백
    public float Power;

    public KnockbackData(float power)
    {
        Power = power;
    }
}

[Serializable]
public struct AttackData
{
    //공격에 대한 정보
    public float Damage; //데미지
    public Vector3 HitDirection; //공격 방향 
    public GameObject Attacker;  //공격의 주체
    public KnockbackData Knockback;

    public AttackData(float damage, Vector3 hitDirection, GameObject attacker, KnockbackData knockback)
    {
        Damage = damage;
        HitDirection = hitDirection;
        Attacker = attacker;
        Knockback = knockback;
    }
}

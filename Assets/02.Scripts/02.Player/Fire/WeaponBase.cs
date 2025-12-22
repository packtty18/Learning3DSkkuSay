
using UnityEngine;

public enum WeaponType
{ 
    Gun,    //총
    Bomb,   //폭탄
    Melee,  //근접
}

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField]protected PlayerController Controller;
    protected PlayerStat Stat => Controller.Stat;
    protected Animator Animator => Controller.Animator;

    public bool IsZoomable = false;

    public virtual void Init(PlayerController controller)
    {
        Controller = controller;
    }

    public abstract bool CanAttack();
    public abstract void Attack();
    public abstract void AttackEnd();
    public abstract void Reload();
}

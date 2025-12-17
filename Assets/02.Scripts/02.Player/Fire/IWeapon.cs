
using UnityEngine;
using UnityEngine.InputSystem.XR;

public enum WeaponType
{ 
    Gun,    //총
    Bomb,   //폭탄
    Melee,  //근접
}

public interface IWeapon
{
    void Select();
    void Deselect();

    void Attack(); 
    void Reload(); 

    bool CanAttack();
}
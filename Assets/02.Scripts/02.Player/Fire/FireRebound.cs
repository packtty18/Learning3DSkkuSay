using DG.Tweening;
using System;
using UnityEngine;

public class FireRebound : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private PlayerStat _stat;
    RecoilData recoil => _stat.CurrentGun.RecoilData;


    public static event Action<RecoilData> OnRecoil;

    public void PlayRebound()
    {
        OnRecoil?.Invoke(recoil);
    }
}



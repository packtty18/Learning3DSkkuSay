using DG.Tweening;
using System;
using UnityEngine;

public class FireRebound : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private PlayerStat _stat;

    private float _recoilX => _stat.GetValue(EValueFloat.GunRecoilX).Value;
    private float _recoilY => _stat.GetValue(EValueFloat.GunRecoilY).Value;


    public static event Action<RecoilData> OnRecoil;

    public void PlayRebound()
    {
        OnRecoil?.Invoke(new RecoilData(_recoilY, _recoilX));
    }
}



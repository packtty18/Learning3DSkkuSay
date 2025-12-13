using DG.Tweening;
using System;
using UnityEngine;

public class FireRebound : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private float _recoilUp = 1f;        // 카메라가 위로 튀는 정도
    [SerializeField] private float _recoilSide = 1f;   // 카메라 뒤로 밀리는 값


    public static event Action<RecoilData> OnRecoil; // 모든 스테이트가 받을 수 있게 이벤트 전달

    public void PlayRebound()
    {
        RecoilData data = new RecoilData(_recoilUp, _recoilSide);
        OnRecoil?.Invoke(data);

        Debug.Log("[Recoil] recoil event fired");
    }
}



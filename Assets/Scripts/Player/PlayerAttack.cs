using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerAttack : MonoBehaviourPun
{
    // 하위 스크립트에서도 사용가능하게 protected

    protected Animator animator;
    protected bool input;
    
    protected void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Input 입력은 부모 스크립트에서 받기
    protected void Update()
    {
        if (!photonView.IsMine) return;

        input = Input.GetMouseButtonDown(0);
    }
}
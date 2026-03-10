using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerAttack : MonoBehaviourPun
{
    // 하위 스크립트에서도 사용가능하게 protected

    protected Animator animator;
    protected bool input; // 마우스 기본 공격
    protected bool commandE;
    protected bool commandQ;
    protected bool shift;
    protected void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Input 입력은 부모 스크립트에서 받기
    protected void Update()
    {
        if (!photonView.IsMine) return;

        input = Input.GetMouseButtonDown(0);
        
        // 스킬 q,e로
        // 눌렀을 때 
        commandE = Input.GetKeyDown(KeyCode.E);
        commandQ = Input.GetKeyDown(KeyCode.Q);
        // 사용할지도 모름 걷기 뛰기 분리할 경우 
        shift = Input.GetKey(KeyCode.LeftShift);
    }
}
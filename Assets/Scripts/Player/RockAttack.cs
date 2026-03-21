using Photon.Pun;
using UnityEngine;

public class RockAttack : PlayerAttack
{
    protected bool isShieldActive = false;
    protected bool shieldInput;

    private RockAnimMover ram;

    
    void Start()
    {
        base.Start();
        ram = GetComponentInChildren<RockAnimMover>();
    }
    void Update()
    {
        base.Update();
        if (input)
        {
            photonView.RPC("AttackAnim", RpcTarget.All);
        }

        shieldInput = Input.GetMouseButton(1);
        // 버튼 상태가 이전과 달라졌을 때만 RPC 전송 (꾹 누르고 있을 때 매 프레임 호출 방지)
        if (shieldInput != isShieldActive)
        {
            isShieldActive = shieldInput;
            // RPC를 통해 모든 클라이언트에게 현재 상태(true/false)를 전달
            photonView.RPC("SyncShieldState", RpcTarget.All, isShieldActive);
        }
    }

    [PunRPC]
    public void AttackAnim()
    {
        animator.SetTrigger("Attack");
    }

    [PunRPC]
    public void SyncShieldState(bool state)
    {
        animator.SetBool("Dif", state);
        if (state == false)
        {
            ram.OffShield();
        }
    }
}
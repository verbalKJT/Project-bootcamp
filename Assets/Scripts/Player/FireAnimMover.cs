using Photon.Pun;
using UnityEngine;

public class FireAnimMover : MonoBehaviour
{
    private Sword sword; // sword.cs
    private PlayerMovement playerMovement;
    private FireManAttack fm;

    void Start()
    {
        sword = GetComponentInChildren<Sword>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        fm = GetComponentInParent<FireManAttack>();
    }

    public void CallForce()
    {
        // 애니메이션 이벤트로 호출
        sword.ShotForce();
    }

    public void OnMove()
    {
        // 움직일 수 있게
        if (!playerMovement.photonView.IsMine) return;
        fm.animator.SetBool("IsCast", false);
        fm.isCast = false;
        playerMovement.canMove = true;
        playerMovement.photonView.RPC("OnMoveRPC", RpcTarget.All);
    }

    public void StopMoveDuringDash()
    {
        if (!playerMovement.photonView.IsMine) return;
        playerMovement.canMove = true;
        playerMovement.col.isTrigger = false;
    }
}
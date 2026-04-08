using Photon.Pun;
using UnityEngine;

public class FireAnimMover : MonoBehaviourPun
{
    private Sword sword; // sword.cs
    private PlayerMovement playerMovement;
    private FireManAttack fm;
    private FireFeedback feedback;
    void Start()
    {
        sword = GetComponentInChildren<Sword>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        fm = GetComponentInParent<FireManAttack>();
        feedback = GetComponentInParent<FireFeedback>();
    }

    public void CallForce()
    {
        // 애니메이션 이벤트로 호출
        sword.ShotForce();
        // 로컬로 오디오 호출
        feedback.ForceFireSound();
    }

    public void OnMove()
    {
        fm.animator.SetBool("IsCast", false);
        // 움직일 수 있게
        if (!playerMovement.photonView.IsMine) return;
        fm.isCast = false;
        playerMovement.canMove = true;
        playerMovement.photonView.RPC("OnMoveRPC", RpcTarget.All);
    }

    public void StopMoveDuringDash()
    {
        if (!playerMovement.photonView.IsMine) return;
        if (!fm.isCast)
        {
            playerMovement.canMove = true;
        }
    }

    public void StartAttack()
    {
        feedback.BasicShotSound(); // 기본 공격 오디오
        if(!photonView.IsMine) return;
        sword.isStrike = true;
        sword.hitTargets.Clear();
        sword.prevPos = sword.center.position;
    }

    public void EndAttack()
    {
        if(!photonView.IsMine) return;
        sword.isStrike = false;
    }
    
}
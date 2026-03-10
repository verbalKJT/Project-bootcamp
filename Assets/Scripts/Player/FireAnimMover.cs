using Photon.Pun;
using UnityEngine;

public class FireAnimMover : MonoBehaviour
{
    private Sword sword; // sword.cs
    private PlayerMovement playerMovement;
    void Start()
    {
        sword = GetComponentInChildren<Sword>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    public void CallForce()
    {
        // 애니메이션 이벤트로 호출
        sword.ShotForce();
    }

    public void OnMove()
    {
        // 움직일 수 있게
        playerMovement.enabled = true;
        playerMovement.photonView.RPC("OnMoveRPC", RpcTarget.All);
    }
    
}

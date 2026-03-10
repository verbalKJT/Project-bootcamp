using System.Collections;
using Photon.Pun;
using UnityEngine;

public class FireManAttack : PlayerAttack
{
    [Header("Sword.cs")] [SerializeField] private Sword sword;

    private PlayerMovement playerMovement;
    private float coolTime = 0f;
    private float forceTime = 5f; // 검기 쿨타임 
    
    
    void Start()
    {
        base.Start();// 부모 Start 메서드 먼저 -> animator 할당
        playerMovement = GetComponent<PlayerMovement>();
    }
    
    void Update()
    {
        base.Update(); // 부모 Update 메서드 먼저 -> Input 입력
        coolTime += Time.deltaTime; // 시간 새키
        if (input)
        {
            StartCoroutine(sword.IncreaseSword(1f));
            photonView.RPC("AttackAnim",RpcTarget.All);
        }

        if (commandE && coolTime >= forceTime) 
        {
            coolTime = 0f; // 쿨타임 초기화 
            photonView.RPC("Force",RpcTarget.All);
        }
    }
    

    [PunRPC]
    public void AttackAnim()
    {
        animator.SetTrigger("Attack");
    }

    [PunRPC]
    public void Force()
    {
        if(!photonView.IsMine) return;
        // 못 움직이도록
        playerMovement.enabled = false;
        animator.SetBool("IsCast",true);
        
        // 애니메이션 이벤트로 플레이어 이동 활성화
    }
}
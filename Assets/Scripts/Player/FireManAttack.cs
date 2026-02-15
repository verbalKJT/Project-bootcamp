using System.Collections;
using Photon.Pun;
using UnityEngine;

public class FireManAttack : PlayerAttack
{
    [Header("Sword.cs")] [SerializeField] private Sword sword;
    
    void Start()
    {
        base.Start();// 부모 Start 메서드 먼저 -> animator 할당
    }
    
    void Update()
    {
        base.Update(); // 부모 Update 메서드 먼저 -> Input 입력
        if (input)
        {
            StartCoroutine(sword.IncreaseSword(1f));
            photonView.RPC("AttackAnim",RpcTarget.All);
        }
    }
    

    [PunRPC]
    public void AttackAnim()
    {
        animator.SetTrigger("Attack");
    }
}
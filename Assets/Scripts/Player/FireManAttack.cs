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
        
        // Q 키 스킬 (휠윈드) 추가
        if (Input.GetKeyDown(KeyCode.Q))
        {
            photonView.RPC("Skill_WheelWin", RpcTarget.All);
        }
    }
    
    [PunRPC]
    public void AttackAnim()
    {
        animator.SetTrigger("Attack");
    }
    
    [PunRPC]
    public void Skill_WheelWin()
    {
        animator.SetTrigger("WheelWin");
        // 휠윈드는 회전하면서 여러 번 때려야 하므로 별도의 코루틴이나 로직이 필요할 수 있습니다.
        // 우선은 기본 Sword 코루틴을 재활용하거나 새로 만듭니다.
        StartCoroutine(sword.IncreaseSword(0.2f)); 
    }
}
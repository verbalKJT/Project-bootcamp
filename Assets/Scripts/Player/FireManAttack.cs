using System.Collections;
using Photon.Pun;
using UnityEngine;

public class FireManAttack : PlayerAttack
{
    [Header("Sword.cs")] [SerializeField] private Sword sword;
    
    
    public float fCoolTime = 0f;
    public float forceTime = 5f; // 검기 쿨타임 
    
    public float dashTime = 3f;
    public float dashCoolTime = 0f; // 대쉬 쿨타임 
    private float dashSpeed = 40f; // 대쉬 속도
    
    [Header("스킬 UI 연걀")]
    [SerializeField] private SkillSlotUI dash;
    [SerializeField] private SkillSlotUI force;
    void Start()
    {
        base.Start();// 부모 Start 메서드 먼저 -> animator + PlayerMovement 할당
    }
    
    void Update()
    {
        base.Update(); // 부모 Update 메서드 먼저 -> Input 입력
        
        if (input)
        {
            StartCoroutine(sword.IncreaseSword(1f));
            photonView.RPC("AttackAnim",RpcTarget.All);
        }

        if (commandE && fCoolTime >= forceTime) 
        {
            fCoolTime = 0f; // 쿨타임 초기화 
            photonView.RPC("Force",RpcTarget.All);
        }
        fCoolTime += Time.deltaTime; // 검기 시간 새기
        force.UpdateSkillSlot(fCoolTime,forceTime); // 스킬 UI 갱신
        
        // 대쉬 스킬은 Shift로 + 땅에 있을 때만?
        if (shift && dashCoolTime >= dashTime && pm.isGrounded)
        {
            dashCoolTime = 0f;
            photonView.RPC("Dash",RpcTarget.All);
        }
        dashCoolTime += Time.deltaTime;// 대쉬 스킬 시간 재기 
        dash.UpdateSkillSlot(dashCoolTime, dashTime);
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
        pm.canMove = false;
        animator.SetBool("IsCast",true);
        
        // 애니메이션 이벤트로 플레이어 이동 활성화
    }

    [PunRPC]
    public void Dash()
    {
        if (!photonView.IsMine) return;
        // 불맨은 캡슐 콜라이더 씀
        pm.col.isTrigger = true; // Dash 할 때만 isTrigger 켜놓기
        pm.canMove = false;
        Slide(dashSpeed);
        animator.SetTrigger("Slide");
        // 애니메이션 이벤트로 플레이어 이동 활성화 + IsTrigger 해제
        pm.col.excludeLayers += LayerMask.NameToLayer("Monster");
    }
    
    // 대쉬 스킬
    public void Slide(float dashSpeed)
    {
        // 방향만 
        Vector3 slideDir = (moveV + moveH).normalized;
        
        if (slideDir != Vector3.zero)
        {
            pm.rb.AddForce(slideDir * dashSpeed, ForceMode.Impulse);
        }
        else
        {
            pm.rb.AddForce(transform.forward * dashSpeed, ForceMode.VelocityChange);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            LivingEnitiy target = other.GetComponent<EnemyHealth>();
            target.TakeDamage(40); // 일단 40뎀 정도만
            target.photonView.RPC("Is_Hit", RpcTarget.All); 
        }
    }
    
}
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class FireManAttack : PlayerAttack
{
    [Header("Sword.cs")] [SerializeField] private Sword sword;
    private float swordReload = 0;
    private float swordCooldown;
    
    
    private float forceTime = 7f;
    private const float fCoolTime = 7f; // 검기 쿨타임 
    public bool isCast = false;
    public bool isDash = false;
    public bool IsActionLocked => isCast || isDash;

    private float dashTime = 5f;
    private const float dashCoolTime = 5f; // 대쉬 쿨타임 
    private float dashDis = 20f; // 대쉬 거리
    private int dashDamage = 30; // 기본 공격이 25
    public override float FirstSkillTime => dashTime;
    public override float FirstSkillCool => dashCoolTime;
    public override float SecSkillTime => forceTime;
    public override float SecSkillCool => fCoolTime;

    private FireFeedback _feedback;

    void Start()
    {
        base.Start(); // 부모 Start 메서드 먼저 -> animator + PlayerMovement 할당
        _feedback = GetComponent<FireFeedback>();
        swordCooldown = sword.weaponData.reloadTime;
    }

    void Update()
    {
        base.Update(); // 부모 Update 메서드 먼저 -> Input 입력
        if (GameManager.isCinematic) return;
        if (input && !IsActionLocked)
        {
            if (swordReload >= swordCooldown)
            {
                photonView.RPC("AttackAnim", RpcTarget.All);
                swordReload = 0;
            }
        
        }
        swordReload += Time.deltaTime;
        if (commandE && forceTime >= fCoolTime && !isDash)
        {
            forceTime = 0f; // 쿨타임 초기화 
            isCast = true;
            photonView.RPC("Force", RpcTarget.All);
        }

        forceTime += Time.deltaTime; // 검기 시간 새기
        // 스킬 UI 갱신

        // 대쉬 스킬은 Shift로 + 땅에 있을 때만?
        if (shift && dashTime >= dashCoolTime && pm.isGrounded && !isCast && !isDash)
        {
            dashTime = 0f;
            isDash = true;
            photonView.RPC("Dash", RpcTarget.All);
        }

        dashTime += Time.deltaTime; // 대쉬 스킬 시간 재기 
    }


    [PunRPC]
    public void AttackAnim()
    {
        animator.SetTrigger("Attack");
    }

    [PunRPC]
    public void Force()
    {
        animator.ResetTrigger("Attack");
        animator.SetBool("IsCast", true);
        _feedback.ForceEffect(); // 아우라 효과
        if (!photonView.IsMine) return;
        // 못 움직이도록
        pm.canMove = false;

        // 애니메이션 이벤트로 플레이어 이동 활성화
    }

    [PunRPC]
    public void Dash()
    {
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Slide");
        _feedback.DashSound();
        Vector3 spawnPos = transform.position - transform.forward * 1.5f;
        _feedback.DashEffect(spawnPos, transform.rotation);
        if (!photonView.IsMine) return;
        // 불맨은 캡슐 콜라이더 씀
        pm.canMove = false;
        Slide(dashDis);
        // 애니메이션 이벤트로 플레이어 이동 활성화 + IsTrigger 해제
        pm.col.excludeLayers |= 1 << LayerMask.NameToLayer("Monster");
    }

    // 대쉬 스킬
    public void Slide(float dashDisance)
    {
        // 대쉬할 방향만 
        Vector3 slideDir = (moveV + moveH).normalized;
        if (slideDir == Vector3.zero) slideDir = transform.forward;

        // 0.3초 동안 이동 
        StartCoroutine(DashRoutine(slideDir, dashDis, 0.3f));
    }

    IEnumerator DashRoutine(Vector3 dir, float distance, float duration)
    {
        // 대쉬 동안 충돌 될 몬스터 갯수
        HashSet<int> monsters = new HashSet<int>();

        pm.canMove = false; // 대쉬 중 조작 금지

        // 대쉬 시작
        Vector3 startPos = pm.rb.position;
        // 대쉬할 거리
        Vector3 targetPos = startPos + dir * distance;

        // 비율
        float elapsed = 0f;

        // 내 콜라이더 크가
        float radius = pm.col.radius;

        while (elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = elapsed / duration; // 0에서 1로 변하는 비율

            // 점진적으로 목표 지점까지 이동 보간
            Vector3 nextPos = Vector3.Lerp(startPos, targetPos, t);
            pm.rb.MovePosition(nextPos);

            LayerMask enemyLayer = LayerMask.GetMask("Monster", "Boss");
            // 맞은 놈 
            Collider[] monColliders = Physics.OverlapSphere(transform.position, radius, enemyLayer);

            foreach (Collider hits in monColliders)
            {
                // 맞은 놈 포톤 뷰 아이디 가져워서 처리 (동기화)
                int hitId = hits.gameObject.GetComponent<PhotonView>().ViewID;
                // 맞은놈 인스터스 아이디 저장 
                if (!monsters.Contains(hitId))
                {
                    monsters.Add(hitId);
                    if (hits.TryGetComponent(out LivingEnitiy target))
                    {
                        target.TakeDamage(dashDamage); // 30데미지
                        if (target.gameObject.layer == LayerMask.NameToLayer("Monster"))
                        {
                            target.photonView.RPC("Is_Hit", RpcTarget.All);
                        }
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }

        // 최종 위치 고정 및 상태 복구
        pm.rb.MovePosition(targetPos);
        isDash = false;
        pm.canMove = !(isCast || isDash);
    }
}
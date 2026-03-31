using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyNearAttack : MonoBehaviourPun
{
    // 공격력은 ScriptableObject로 -> EnemyHealth에 있음
    private MonsterState foreast_state;
    private int damage = 100; // 근접 공경력

    private float nearAttackRange = 8f; // 근거리 공격 범위
    private float nearAttackTime = 5f; // 5초바다 공격
    private float nearAttackCool = 5f;

    // 타격 판정 설정
    [SerializeField] private Transform strikePoint; // 공격 시작위치(오른손)
    private float strikeRadius = 1.5f; // 오른손 기준 영역 (구)
    private bool isStrike = false;
    private Vector3 previousPos; // 공격 중 오른손의 이전 위치를 저장할 변수
    
    // 맞은 플레이어 확인 용
    private HashSet<Collider> strikeTargets = new HashSet<Collider>();
    
    // 가장 가까운 타겟(플레이어)
    private bool target_Died = false;

    private EnemySensor _enemySensor;
    private EnemyHealth _enemyHealth;
    private EnemyRangeAttack _enemyRangeAttack;

    void Start()
    {
        foreast_state = GetComponent<EnemyHealth>().foreast_state;
        damage = foreast_state.nearDam;
        _enemySensor = GetComponent<EnemySensor>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemyRangeAttack = GetComponent<EnemyRangeAttack>();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        nearAttackTime += Time.fixedDeltaTime; // 공격 쿨 재기

        if (_enemySensor.curTarget != null && _enemySensor.distanceToTarget <= nearAttackRange)
        {
            // 처다보면서 걷도록 -> 이상할 듯 ㅇㅇ
            transform.LookAt(_enemySensor.curTarget);
            // 근접 공격 범위 내면 원거리 공격 안하도록
            _enemyRangeAttack.enabled = false;
            target_Died = _enemySensor.curTarget.GetComponent<PlayerHealth>().isDead;
            if (!target_Died)
            {
                // 공격 시간은 4~6.5 사이
                if (nearAttackTime >= nearAttackCool)
                {
                    photonView.RPC("NearAttack", RpcTarget.All, true);
                    nearAttackTime = 0f;
                }
            }
        }
        else // 근접 공격 범위를 벗어 낫으니
        {
            _enemyRangeAttack.enabled = true;
        }
    }

    [PunRPC]
    public void NearAttack(bool state)
    {
        _enemyHealth.animator.SetBool("Near", state);
    }

    void Update()
    {
        // 애니메이션 시작 시 
        if(!PhotonNetwork.IsMasterClient && !isStrike) return;
        
        CheckStrikeBound();
    }
    
    public void StartNearHit()
    {
        if(!PhotonNetwork.IsMasterClient) return;

        isStrike = true;
        previousPos = strikePoint.position; // 궤도 추적 시작점
        strikeTargets.Clear(); // 새로운 공격
    }

    public void EndNearHit()
    {
        if(!PhotonNetwork.IsMasterClient) return;
        
        isStrike = false;
        photonView.RPC("NearAttack", RpcTarget.All, false);
    }

    public void CheckStrikeBound()
    {
        Vector3 currentPos = strikePoint.position;
        
        // 프레임간 오른손의 이동 거리
        Vector3 direction = strikePoint.position - previousPos;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            // 이전 위치에서 현재 위치 방향으로 거리만큼 구체를 쏴서 부딪힌 모든 것을 가져옴
            RaycastHit[] hits = Physics.SphereCastAll(previousPos,strikeRadius,
                direction.normalized,distance,LayerMask.GetMask("Player"));
            foreach (RaycastHit hit in hits)
            {
                Collider hitCol =  hit.collider;
                
                // 이미 맞은 놈이면 넘어감
                if(strikeTargets.Contains(hitCol)) continue;
                
                // 맞은 놈 처리
                strikeTargets.Add(hitCol);
                
                LivingEnitiy pH =  hit.collider.GetComponent<PlayerHealth>();
                // 공격 처리
                pH.TakeDamagePlayer(damage);
            }
        }
        // 이동한 위치 덮어 쓰기
        previousPos = currentPos;
    }
    private void OnDrawGizmosSelected()
    {
        if (strikePoint != null)
        {
            Gizmos.color = new Color(0, 1, 1, 0.5f);
            Gizmos.DrawWireSphere(strikePoint.position, strikeRadius);
        }
    }
}
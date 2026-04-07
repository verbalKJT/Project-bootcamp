using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyNearAttack : MonoBehaviourPun
{
    // 공격력은 ScriptableObject로 -> EnemyHealth에 있음
    private MonsterState foreast_state;
    private int damage; // 근접 공경력

    private float nearAttackRange = 9f; // 근거리 공격 범위
    private float nearAttackTime = 3f; // 3초마다공격
    private float nearAttackCool = 3f;

    // 타격 판정 설정
    [SerializeField] private Transform strikePoint; // 공격 시작위치(오른손)
    private float strikeRadius = 1.5f; // 오른손 기준 영역 (구)
    private bool isStrike = false;
    private Vector3 previousPos; // 공격 중 오른손의 이전 위치를 저장할 변수

    // 맞은 플레이어 확인 용
    private HashSet<int> strikeTargets = new HashSet<int>();

    // 가장 가까운 타겟(플레이어)
    private bool target_Died = false;

    private EnemySensor _enemySensor;
    private EnemyHealth _enemyHealth;
    private EnemyRangeAttack _enemyRangeAttack;
    private EnemyMove _enemyMove;
    private MonsterFeedBack feedback;

    void Start()
    {
        foreast_state = GetComponent<EnemyHealth>().foreast_state;
        damage = foreast_state.nearDam;
        _enemyMove = GetComponent<EnemyMove>();
        _enemySensor = GetComponent<EnemySensor>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemyRangeAttack = GetComponent<EnemyRangeAttack>();
        feedback = GetComponent<MonsterFeedBack>();

        damage = _enemyMove._monsterState.nearDam;
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        // obj 파괴 or boss Clear
        if(CriticalObj.isDestroyed || BossHp.BossDead)return;
        if (GameManager.isCinematic) return;
        nearAttackTime += Time.fixedDeltaTime; // 공격 쿨 재기

        if (_enemySensor.curTarget != null && _enemySensor.distanceToTarget <= nearAttackRange)
        {
            // 처다보면서 걷도록 -> 이상할 듯 ㅇㅇ
            transform.LookAt(_enemySensor.curTarget);
            // 근접 공격 범위 내면 원거리 공격 안하도록
            _enemyRangeAttack.enabled = false;

            if (_enemySensor.isCriticalTarget)
            {
                // criticalObj를 찾았다면
                CriticalObj cri = _enemySensor.curTarget.GetComponent<CriticalObj>();
                target_Died = (cri == null || cri.curHp <= 0);
            }
            else
            {
                // criticalObj를 못 찾았다면
                PlayerHealth player = _enemySensor.curTarget.GetComponent<PlayerHealth>();
                target_Died = (player == null || player.isDead);
            }

            if (!target_Died)
            {
                if (nearAttackTime >= nearAttackCool)
                {
                    photonView.RPC("NearAttack", RpcTarget.All, true);
                    nearAttackTime = 0f; // 쿨타임 초기화.
                    if (_enemySensor.isCriticalTarget)
                    {
                        // CriticlaObj(몬스터 최우선 공격 목표)를 찾은 상태면
                        _enemyMove.agent.isStopped = true;
                    }
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
        if (state)
            feedback.CallNearClip();
        _enemyHealth.animator.SetBool("Near", state);
    }

    void Update()
    {
        // 애니메이션 시작 시 
        if (!PhotonNetwork.IsMasterClient || !isStrike) return;

        if (GameManager.isCinematic) return;
        CheckStrikeBound();
    }

    // 애니메이션 이벤트
    public void StartNearHit()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        isStrike = true;
        previousPos = strikePoint.position; // 궤도 추적 시작점
        _enemyMove.agent.isStopped = true; // 공격하는 동안 걷지 않기
        strikeTargets.Clear(); // 새로운 공격
    }

    // 애니메이션 이벤트
    public void EndNearHit()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        isStrike = false;
        _enemyMove.agent.isStopped = false; // 다시 목표를 향해 걷도록
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
            int hitMask = LayerMask.GetMask("Player", "CriticalObj");

            // 이전 위치에서 현재 위치 방향으로 거리만큼 구체를 쏴서 부딪힌 모든 것을 가져옴
            RaycastHit[] hits = Physics.SphereCastAll(previousPos, strikeRadius,
                direction.normalized, distance, hitMask);
            foreach (RaycastHit hit in hits)
            {
                Collider hitCol = hit.collider;

                if (_enemySensor.isCriticalTarget)
                {
                    CriticalObj cri = hitCol.GetComponent<CriticalObj>();

                    if (cri == null) continue;

                    int hitViewId = cri.photonView.ViewID;

                    // 이미 맞은 놈이면 넘어감
                    if (strikeTargets.Contains(hitViewId)) continue;

                    // 맞은 놈 처리
                    strikeTargets.Add(hitViewId);
                    cri.TakeDamageObj(damage);
                }
                else
                {
                    LivingEnitiy pH = hitCol.GetComponent<PlayerHealth>();

                    if (pH == null) continue;

                    int hitViewId = pH.photonView.ViewID;

                    // 이미 맞은 놈이면 넘어감
                    if (strikeTargets.Contains(hitViewId)) continue;

                    // 맞은 놈 처리
                    strikeTargets.Add(hitViewId);
                    pH.TakeDamage(damage);
                }
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
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class EnemyRangeAttack : MonoBehaviourPun
{
    // 몬스터의 공격은 2가지
    // 원거리, 근거리 
    // 근거리 -> 몬스터의 목표물을 우선 타겟
    // 원거리 -> 바위맨 쉴드에 막혀야함. 바위맨 쉴드에 체력 필요
    private float rangedAttackRange = 20f; // 원거리 공격 범위
    private float rangedAttackCool; // 원거리 공격 텀 약 4~6.5초
    private float rangedAttack = 4f; // 공격 시간 잴 변수 

    // 원거리 공격이 나갈 위치
    [field: SerializeField] public Transform firePos { get; private set; }

    // 원거리 공격 던질 Obj
    [field: SerializeField] public GameObject weapon { get; private set; }

    // 생성된 무기
    private GameObject fire;
    private Rock fireRock;


    private bool target_Died = false;

    // 원거리 공격 대상 여부
    private bool hasRangeTarget = false;

    private EnemyMove em;
    private EnemyHealth _enemyHealth;
    private EnemySensor _enemySensor;
    private MonsterFeedBack feedback;

    private void ClearProjectileRefs()
    {
        fire = null;
        fireRock = null;
    }

    void Start()
    {
        em = GetComponent<EnemyMove>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemySensor = GetComponent<EnemySensor>();
        feedback = GetComponent<MonsterFeedBack>();
        RandomCool(); // 공격 쿨 갱신
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        // obj 파괴 or boss Clear
        if(CriticalObj.isDestroyed || BossHp.BossDead)return;
        if(CriticalObj.isDestroyed) return;
        rangedAttack += Time.fixedDeltaTime;
        // CriticalObj를 찾았다면 원거리 공격 안함.
        if (_enemySensor.isCriticalTarget) return;
        if (_enemySensor.curTarget != null && _enemySensor.distanceToTarget <= rangedAttackRange)
        {
            target_Died = _enemySensor.curTarget.GetComponent<PlayerHealth>().isDead;
            if (!target_Died)
            {
                // 공격 시간은 4~6.5 사이
                if (rangedAttack >= rangedAttackCool)
                {
                    photonView.RPC("RangeAttack", RpcTarget.All, true);
                    rangedAttack = 0f;
                    RandomCool(); // 공격 쿨타임 갱신
                }
            }
        }
    }

    [PunRPC]
    private void RangeAttack(bool state)
    {
        if (state)
            feedback.CallRangeClip();
        _enemyHealth.animator.SetBool("RangeAttack", state);
    }

    // 애니메이션 이벤트로 호출 -> 애니메이션이 RPC로 동기화 되기 때문에 이 함수 자체는 RPC가 아니여도 됨.
    public void MakeFire()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (_enemySensor.curTarget == null)
        {
            photonView.RPC("RangeAttack", RpcTarget.All, false);
            ClearProjectileRefs();
            return;
        }

        if (fire == null || fireRock == null)
        {
            ClearProjectileRefs();
        }

        // 몬스터의 ViewId를 담아서
        object[] instantiationData = new object[] { photonView.ViewID };
        // 생성
        fire = PhotonNetwork.InstantiateRoomObject("Enemies/" + weapon.name, firePos.position, firePos.rotation, 0,
            instantiationData);
        if (fire == null)
        {
            photonView.RPC("RangeAttack", RpcTarget.All, false);
            ClearProjectileRefs();
            return;
        }

        fireRock = fire.GetComponent<Rock>();
        if (fireRock == null)
        {
            photonView.RPC("RangeAttack", RpcTarget.All, false);
            if (PhotonNetwork.IsMasterClient && fire != null)
            {
                PhotonNetwork.Destroy(fire);
            }

            ClearProjectileRefs();
        }
    }

    public void Shoot()
    {
        // 마스터 클라이언트만 발사
        if (!PhotonNetwork.IsMasterClient) return;
        // 타겟이 범위를 벗어나는 순간 애니메이션 변화
        if (_enemySensor.curTarget == null)
        {
            photonView.RPC("RangeAttack", RpcTarget.All, false);
            // 안 던지니까 삭제
            if (fire != null)
            {
                PhotonNetwork.Destroy(fire);
            }

            ClearProjectileRefs();
            return;
        }

        if (fire == null || fireRock == null)
        {
            photonView.RPC("RangeAttack", RpcTarget.All, false);
            ClearProjectileRefs();
            return;
        }

        // 부모 해제
        fire.transform.SetParent(null);
        // 타겟까지의 방향
        Vector3 direction = (_enemySensor.curTarget.position - firePos.position).normalized;

        // 타겟의 위치를 40m 정도 뒤로 생각 -> 아래 최소한의 힘이 강해짐 
        Vector3 sTargert = _enemySensor.curTarget.position + direction * 40f;

        // 포물선 공식을 사용한 초기 속도 계산 -> 목표지점까지의 최소한의 힘임       
        Vector3 reVelocity = CalculateTargetPosition(firePos.position,
            sTargert, 5f); // angle 각도가 작아질 수로 Cos값이 커져서 힘이 세짐.


        // 던질 때 중력 적용
        fireRock.rb.useGravity = true;
        // 속도를 그대로 대입
        fireRock.rb.linearVelocity = reVelocity;

        // 돌이 날아갈 때 랜덤하게 회전.
        fireRock.rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);

        photonView.RPC("RangeAttack", RpcTarget.All, false);
        ClearProjectileRefs();
    }


    private void RandomCool()
    {
        rangedAttackCool = Random.Range(4f, 6.5f); // 갱신
    }

    // Gizmo를 이용 확인용
    private void OnDrawGizmosSelected()
    {
        // 1. 원의 색상 설정 (반투명한 빨간색)
        Gizmos.color = new Color(1, 0, 0, 0.3f);

        // 2. 속이 꽉 찬 구 그리기 (범위 확인용)
        Gizmos.DrawSphere(transform.position, rangedAttackRange);

        // 3. 테두리 선 그리기 (경계선 확인용)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangedAttackRange);
    }

    // 곡선의 방정식 이용 -> 타겟 지점에 정확히 떨어지기 위한 "최소한의 힘"
    private Vector3 CalculateTargetPosition(Vector3 start, Vector3 end, float angle)
    {
        // 방향 및 거리
        Vector3 direction = new Vector3(end.x - start.x, 0, end.z - start.z); // 수평 방향 벡터
        float x = direction.magnitude; // 수평 거리
        float y = end.y - start.y; // 수직 높이차

        // 물리 상수 준비
        float gravity = Mathf.Abs(Physics.gravity.y); // 중력 값
        float angleRad = angle * Mathf.Deg2Rad; // 각도를 라디반으로 변환

        // 포물선 공식 사용
        float v_2 = (gravity * x * x) / (2 * (x * Mathf.Tan(angleRad) - y) * Mathf.Pow(Mathf.Cos(angleRad), 2));

        // 예외 처리 수학적으로 도달 불가능한 목표일 때
        if (v_2 <= 0f || float.IsNaN(v_2))
        {
            Debug.LogWarning("v_2 is NaN 도달 불가능");
            return (end - start).normalized * 20f; // 기본 직선 발사
        }

        // 초기 속력
        float v = Mathf.Sqrt(v_2);

        // 최종 속도 벡터 조립
        Vector3 _velocity = direction.normalized * (v * Mathf.Cos(angleRad)); // 수평 속도 성분
        // 성분들의 * 1.5 처럼 직접적으로 힘을 주게 되면 궤적 자체가 바뀜
        _velocity.y = v * Mathf.Sin(angleRad); // 수직 속도 성분

        return _velocity;
    }
}
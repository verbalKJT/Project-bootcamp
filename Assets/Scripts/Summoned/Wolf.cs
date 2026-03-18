using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Wolf : Summoned
{
    public enum State
    {
        Idle,
        Move,
        Attack
    }

    public State _curState { get; private set; }
    public State _befState { get; private set; }
    private FSM _fsm;

    // 상태가 바뀔 때마다 생성하지 않고 만든 객체 재사용
    private IdleState _idleState;
    private MoveState _moveState;
    private AttackState _attackState;

    // 기본 컴포넌트들
    public Animator animator;
    private Rigidbody rb;
    public PhotonView photonView;
    public BoxCollider collider;

    // 자기 플레이어와 플레이어를 따라갈 위치(소환위치)
    private GameObject master;
    private Transform masterTransform; // 초기 위치

    // 늑대 스탯들 -> 스크립터블 오브젝트나 Summoned로 관리해도 됨.
    private float speed = 15f;
    private int damage = 20;
    private float detectRange = 25f; // 탐지 범위
    private float attackRange = 4f; // 공격 가능 범위 
    private float attackTime = 0;
    private float attackCool = 3f;

    private Vector3 _currentVelocity;

    // enemy
    private Collider[] enemy = new Collider[30];
    private Transform target;

    private void Start()
    {
        // FSM 초기화
        _idleState = new IdleState(this);
        _moveState = new MoveState(this);
        _attackState = new AttackState(this);
        _curState = State.Idle;
        _fsm = new FSM(_idleState);
        // 컴포넌트 할당
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        collider = GetComponent<BoxCollider>();
        photonView.RPC("WolfSummoned", RpcTarget.All);

        // 늑대 주인 등록
        master = PlayerMovement.player;

        // 15초뒤 디스폰
        //if (PhotonNetwork.IsMasterClient)
        //    StartCoroutine(DeSpawn(15f));
    }

    // 상태 체크
    private void Update()
    {
        if (!photonView.IsMine) return;
        Debug.Log(_curState);
        switch (_curState)
        {
            case State.Idle:
                if (FindObj()) // 범위 내에서 찾았다면
                {
                    if (CanAttackObj()) // 공격 사거리 내라면
                    {
                        ChangeState(State.Attack);
                    }
                    else
                    {
                        ChangeState(State.Move);
                    }
                }

                break;
            case State.Move:
                if (FindObj()) // 탐지 범위 내 있다
                {
                    if (CanAttackObj()) // 공격 사거리 내이다
                    {
                        ChangeState(State.Attack);
                    }
                }
                else // 탐지 범위를 벗어났다.
                {
                    ChangeState(State.Idle);
                }

                break;
            case State.Attack:
                if (FindObj())
                {
                    if (!CanAttackObj()) // 공격 사거리를 벗어났다.
                    {
                        ChangeState(State.Move);
                    }
                }
                else
                {
                    ChangeState(State.Idle);
                }

                break;
        }

        if (_curState == State.Attack)
        {
            if (attackTime >= attackCool)
            {
                _fsm.UpdateState(); // 각 상태에서 매 프레임마다 실행해야할 로직들
            }
        }
        else
        {
            _fsm.UpdateState();
        }


        attackTime += Time.deltaTime;
    }

    // 상태 변화
    private void ChangeState(State nextState)
    {
        if (_curState == nextState) return; // 같은 상태로의 변화는 무시
        // 이전 상태 저장
        _befState = _curState;
        // 상태 변화
        _curState = nextState;
        switch (_curState)
        {
            case State.Idle:
                _fsm.ChangeState(_idleState);
                break;
            case State.Move:
                _fsm.ChangeState(_moveState);
                break;
            case State.Attack:
                _fsm.ChangeState(_attackState);
                break;
        }
    }

    // 실제 늑대(소환수)가 행할 상태와 상태에 따른 행동
    public override void ExecuteIdle()
    {
        if (!photonView.IsMine) return;

        if (Vector3.Distance(transform.position, masterTransform.position) <= 0.1f)
        {
            // 앉는 애니메이션으로 변경
        }
        else // 플레이어가 이동중이라 같은 위치가 아닐 수밖에 없음
        {
            // ref 참조 덕에 계속 업데이트 됨
            transform.position =
                Vector3.SmoothDamp(transform.position,
                    masterTransform.position, ref _currentVelocity, 0.2f);


            // 플레이어가 보는 방향으로 회전 
            Quaternion targetRot = masterTransform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, speed * Time.deltaTime);
        }
    }

    public override void ExecuteMove()
    {
        if (!photonView.IsMine) return;
        transform.LookAt(target);
        // 부드럽게 이동.
        transform.position =
            Vector3.SmoothDamp(transform.position,
                target.position, ref _currentVelocity, 0.1f, speed);
    }

    public override void ExecuteAttack()
    {
        if (!photonView.IsMine) return;
        
        attackTime = 0; // 공격 쿨타임 초기화
        // 애니메이션 이벤트로 isTrigger 끄고 키기
    }

    private bool FindObj()
    {
        float minDis = Mathf.Infinity;
        // 적 오브젝트 찾기 로직
        // 늑대 기준 탐지 범위 내 적 콜라이더 개수
        int enemiesInBound =
            Physics.OverlapSphereNonAlloc(transform.position, detectRange, enemy, LayerMask.GetMask("Monster"));

        if (enemiesInBound == 0) return false;
        for (int i = 0; i < enemiesInBound; i++)
        {
            float distance = Vector3.Distance(transform.position, enemy[i].transform.position);

            if (distance < minDis)
            {
                minDis = distance;
                target = enemy[i].transform;
            }
        }

        if (target != null)
        {
            return true;
        }

        return false;
    }

    private bool CanAttackObj()
    {
        // 공격 사정거리 체크
        float attackDis = Vector3.Distance(transform.position, target.position);

        return attackDis <= attackRange;
    }

    public void SetSlot(Transform t)
    {
        // 플레이어 양옆 중 하나 늑대마다 다름
        masterTransform = t;
    }

    [PunRPC]
    private void WolfSummoned()
    {
        animator.SetTrigger("Summoned");
    }

    // 충돌로 피격 처리
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            // 공격 처리
            LivingEnitiy target = other.GetComponent<EnemyHealth>();
            target.photonView.RPC("Is_Hit", RpcTarget.All);
            target.TakeDamage(damage);
        }
    }

    IEnumerator DeSpawn(float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void WolfAnimChanger(State state, float ramdom)
    {
        switch (state)
        {
            // Idle은 안 움직일 때니까 필요없음
            // 나중에 가만히 있는 애니메이션 추가 시에 case 추가
            case State.Idle: // Idle이라는건 바운더리 내도 공격가능 사거리 내도 아니니까 
                animator.SetBool("IsBoundary", false);
                animator.SetBool("CanAttack", false);
                break;
            // Move
            case State.Move:
                animator.SetBool("IsBoundary", true);
                animator.SetBool("CanAttack", false);
                break;
            // Attack
            case State.Attack:
                animator.SetBool("CanAttack", true);
                animator.SetFloat("Attack", ramdom);
                break;
        }

        _curState = state;
    }

    public override void PlayEachAnims()
    {
        if (!photonView.IsMine) return;
        float ramdom = Random.Range(0f, 1f); // 0~ 1.0 사이 값 랜덤
        photonView.RPC("WolfAnimChanger", RpcTarget.All, _curState, ramdom);
    }
}
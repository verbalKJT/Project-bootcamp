using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Wolf : Summoned
{
    private enum State{
        Idle,
        Move,
        Attack
    }
    private State _curState;
    private FSM _fsm;
    
    // 상태가 바뀔 때마다 생성하지 않고 만든 객체 재사용
    private IdleState _idleState;
    private MoveState _moveState;
    private AttackState _attackState;
    
    // 기본 컴포넌트들
    private Animator animator;
    private Rigidbody rb;

    // 자기 플레이어와 플레이어를 따라갈 위치(소환위치)
    private GameObject master;
    private Transform masterTransform; // 초기 위치
    
    // 늑대 스탯들
    private float speed = 50f;
    private int damae = 20;
    
    private Vector3 _currentVelocity;
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
        photonView.RPC("WolfSummoned",RpcTarget.All);
        
        // 늑대 주인 등록
        master = PlayerMovement.player;
    }

    // 상태 체크
    private void Update()
    {
        if(!photonView.IsMine) return;
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

        _fsm.UpdateState();
    }
    // 상태 변화
    private void ChangeState(State nextState)
    {
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
            // MoveTowards or SmoothDamp
            //transform.position = Vector3.MoveTowards(transform.position, masterTransform.position, speed * Time.deltaTime);

            // ref 참조 덕에 계속 업데이트 됨
            transform.position =
                Vector3.SmoothDamp(transform.position, 
                    masterTransform.position, ref _currentVelocity, 0.2f);
            
            
            // 플레이어가 보는 방향으로
            Quaternion targetRot = masterTransform.rotation; 
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, speed * Time.deltaTime);
        }
        
    }
    
    public override void ExecuteMove(){}
    public override void ExecuteAttack() {}

    private bool FindObj()
    {
        // 적 오브젝트 찾기 로직
        return false;
    }

    private bool CanAttackObj()
    {
        // 공격 사정거리 체크
        return false;
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
    
}

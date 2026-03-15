using Photon.Pun;

public abstract class Summoned : MonoBehaviourPun
{
    private enum State{
        Idle,
        Move,
        Attack
    }
    private State _state;
	
    private void Start(){
        _state = State.Idle;
    }
	
    private void Update(){
        switch(_state){
            case State.Idle:
                //Idle 행동 구현
                break;
            case State.Move:
                // Move행동 구현
                break;
            case State.Attack : 
                // Attack 행동 구현
                break;
        }
    }
    
    // 각 소환수들이 공통적으로 사용할 상태메서드
    public abstract void ExecuteIdle();
    public abstract void ExecuteMove();
    public abstract void ExecuteAttack();
}

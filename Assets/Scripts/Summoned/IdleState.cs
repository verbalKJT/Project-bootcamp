using UnityEngine;

public class IdleState : BaseState
{
    // 생성자
    public IdleState(Summoned summoned) : base(summoned) {}
    
    public override void EnterState()
    {
        _summoned.ExecuteIdle();
    }
    public override void UpdateState()
    {
        _summoned.ExecuteIdle();
    }

    public override void ExitState()
    {
    }
}

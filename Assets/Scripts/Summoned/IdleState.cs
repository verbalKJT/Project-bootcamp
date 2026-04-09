using UnityEngine;

public class IdleState : BaseState
{
    // 생성자
    public IdleState(Summoned summoned) : base(summoned) {}
    
    public override void EnterState()
    {
        UpdateState();
        _summoned.PlayEachAnims();
    }
    public override void UpdateState()
    {
        _summoned.ExecuteIdle();
    }

    public override void ExitState()
    {
    }
}

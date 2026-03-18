using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(Summoned summoned) : base(summoned)
    {
    }

    public override void EnterState()
    {
        UpdateState();
        _summoned.PlayEachAnims();
    }

    public override void UpdateState()
    {
        _summoned.ExecuteAttack();
    }

    public override void ExitState()
    {
    }
}
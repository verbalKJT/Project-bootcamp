using Photon.Pun;
using UnityEngine;

public class MoveState : BaseState
{
    public MoveState(Summoned summoned) : base(summoned){}

    public override void EnterState()
    {
        UpdateState();
        _summoned.PlayEachAnims();
    }
    public override void UpdateState()
    {
        _summoned.ExecuteMove();
    }

    public override void ExitState()
    {
    }
    
}

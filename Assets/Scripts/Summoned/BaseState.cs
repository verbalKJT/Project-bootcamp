using Photon.Pun;

public abstract class BaseState
{
    protected Summoned _summoned;

    protected BaseState(Summoned summoned)
    {
        _summoned = summoned;
    }
    
    // 상태에 처음 진입했을시 한번만 호출
    public abstract void EnterState();
    // 매 프레임 마다 호출되어 상태 체크
    public abstract void UpdateState();
    // 상태가 변경되면 호출
    public abstract void ExitState();
}

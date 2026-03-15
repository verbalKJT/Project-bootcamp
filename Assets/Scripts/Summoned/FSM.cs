public class FSM
{
    public FSM(BaseState initState)
    {
        _curState = initState;
        ChangeState(_curState);
    }

    private BaseState _curState;

    public void ChangeState(BaseState nextState)
    {
        if (nextState == _curState)
        {
            return;
        }

        if (_curState != null)
        {
            _curState.ExitState();
        }

        _curState = nextState;
        _curState.EnterState();
    }

    public void UpdateState()
    {
        if (_curState != null)
            _curState.UpdateState();
    }
}
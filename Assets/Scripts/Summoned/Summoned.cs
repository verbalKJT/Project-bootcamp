using Photon.Pun;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public abstract class Summoned : MonoBehaviourPun
{
    // 각 소환수들이 공통적으로 사용할 상태메서드
    public abstract void ExecuteIdle();
    public abstract void ExecuteMove();
    public abstract void ExecuteAttack();
    
    // 각 상태 클래스에서 호출할 애니메이션 호출 메서드
    public abstract void PlayEachAnims();
    
}

using UnityEngine;

public class NatureAnimMover : MonoBehaviour
{
    private NatureManAttack natureManAttack;

    
    
    void Start()
    {
        natureManAttack = GetComponentInParent<NatureManAttack>();
    }
    public void NatureCannonFire()
    {
        natureManAttack.Fire();
    }
    
    public void SummonWolf()
    {
        natureManAttack.Summon();
        natureManAttack.animator.SetBool("Summon",false); // 소환후 재소환 안하게 애니메이터 Transition 막기
    }

    public void SummonSnare()
    {
        natureManAttack.SnareAttack();
    }
}

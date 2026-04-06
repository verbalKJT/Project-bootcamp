using UnityEngine;

public class NatureAnimMover : MonoBehaviour
{
    private NatureManAttack natureManAttack;
    private NatureFeedback feedback;
    void Start()
    {
        natureManAttack = GetComponentInParent<NatureManAttack>();
        feedback = GetComponentInParent<NatureFeedback>();
    }
    public void NatureCannonFire()
    {
        natureManAttack.Fire();
        feedback.SnareFireSound();
    }
    
    public void SummonWolf()
    {
        natureManAttack.Summon();
        natureManAttack.animator.SetBool("Summon",false); // 소환후 재소환 안하게 애니메이터 Transition 막기
    }

    public void SummonSnare()
    {
        natureManAttack.SnareAttack();
        feedback.SnareFireSound();
    }
}

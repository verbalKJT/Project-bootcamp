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
    }
}

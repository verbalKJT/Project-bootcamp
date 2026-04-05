using UnityEngine;

public class BossAnimMover : MonoBehaviour
{
    private BossAttack _bossAttack;
    
    void Start()
    {
        _bossAttack = GetComponentInParent<BossAttack>();
    }

    public void CallSpwanCrystal()
    {
        _bossAttack.SpawnCrystal();
    }

    public void CallSpawnElectricWires()
    {
        _bossAttack.SpwanElectric();
    }
}
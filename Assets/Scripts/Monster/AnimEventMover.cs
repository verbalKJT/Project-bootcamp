using Photon.Pun;
using UnityEngine;

public class AnimEventMover : MonoBehaviour
{
    private EnemyRangeAttack ea;

    private GameObject weapon;
    void Start()
    {
        ea = GetComponentInParent<EnemyRangeAttack>();
    }
    public void EndDeadAnim()
    {
        EnemyHealth enemyHealth = GetComponentInParent<EnemyHealth>();
        // 이벤트 발동 시 EnemyHealth의 메소드 호출
        enemyHealth.DestroyObj();
    }

    public void MakeFire()
    {
        ea.MakeFire();
    }

    public void Shoot()
    {
        ea.Shoot();
    }
}
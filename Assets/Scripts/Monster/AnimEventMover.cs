using Photon.Pun;
using UnityEngine;

public class AnimEventMover : MonoBehaviour
{
    private EnemyRangeAttack _enemyRangeAttack;
    private EnemyNearAttack _enemyNearAttack;

    private GameObject weapon;
    void Start()
    {
        _enemyRangeAttack = GetComponentInParent<EnemyRangeAttack>();
        _enemyNearAttack = GetComponentInParent<EnemyNearAttack>();
    }
    public void EndDeadAnim()
    {
        EnemyHealth enemyHealth = GetComponentInParent<EnemyHealth>();
        // 이벤트 발동 시 EnemyHealth의 메소드 호출
        enemyHealth.DestroyObj();
    }

    public void MakeFire()
    {
        _enemyRangeAttack.MakeFire();
    }

    public void Shoot()
    {
        _enemyRangeAttack.Shoot();
    }
    
    
    // 근접 공격.
    // 공격 시작
    public void StartNearHit()
    {
        _enemyNearAttack.StartNearHit();
    }
    // 공격 끝
    public void EndNearHit()
    {
        _enemyNearAttack.EndNearHit();
    }
}
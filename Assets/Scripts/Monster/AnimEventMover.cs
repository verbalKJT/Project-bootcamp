using UnityEngine;

public class AnimEventMover : MonoBehaviour
{
    
    public void EndDeadAnim()
    {
        EnemyHealth enemyHealth = GetComponentInParent<EnemyHealth>();
        // 이벤트 발동 시 EnemyHealth의 메소드 호출
        enemyHealth.DestroyObj();
    }
    
    
    public void OnHammerHit()
    {
        Debug.Log("OnHammerHit");
        Hammer hammer = GetComponentInChildren<Hammer>();
        hammer.OnCollider();
    }

    public void OffHammerHit()
    {
        Debug.Log("OffHammerHit");
        Hammer hammer = GetComponentInChildren<Hammer>();
        hammer.OffCollider();
    }

    
}
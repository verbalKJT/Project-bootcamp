using UnityEngine;

public class AnimEventMover : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    void Start()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }
    
    public void EndDeadAnim()
    {
        // 이벤트 발동 시 EnemyHealth의 메소드 호출
        enemyHealth.DestroyObj();
    }
}

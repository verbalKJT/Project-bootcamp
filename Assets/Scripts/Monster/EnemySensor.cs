using System.Collections;
using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    private float rangedAttackRange = 20f; // 원거리 공격 범위

    // 원거리 공격 대상 여부
    public bool hasRangeTarget = false;
    


    private EnemyRangeAttack eRa;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eRa = GetComponent<EnemyRangeAttack>();
        StartCoroutine(FindPlayerRoutine(0.4f));
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator FindPlayerRoutine(float delay)
    {
        while (true)
        {
            // 0.4초 마다 가장 가까운 플레이어 찾기
            FindNearestPlayer();
            yield return new WaitForSeconds(delay);
        }
    }
    
    private void FindNearestPlayer()
    {
        // 공격 거리 기준 가상 구
        Collider[] players = Physics.OverlapSphere(transform.position, rangedAttackRange, LayerMask.GetMask("Player"));
        float minDistance = float.MaxValue;
        // 초기화가 됨.
        Transform nearest = null;

        foreach (Collider player in players)
        {
            // Distance -> 제곱근까지 사용해서 CPU 낭비가 큼 / sqrMagnitude은 Distance에서 제곱근만 사용안함.
            float distToPlayer = (transform.position - player.transform.position).sqrMagnitude;

            if (distToPlayer < minDistance)
            {
                minDistance = distToPlayer;
                nearest = player.transform;
            }
        }

        // 공격 범위 내 가장 가까운 플레이어
        eRa.target = nearest;

        hasRangeTarget = (eRa.target != null) ? true : false;
    }
}
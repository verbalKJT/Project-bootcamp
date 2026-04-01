using System.Collections;
using Photon.Pun;
using UnityEngine;

public class EnemySensor : MonoBehaviourPun
{
    private float detectRange = 30f; // 몬스터의 플레이어 탐지 범위
    private float detectCriticlaRange = 10f; // 몬스터의 플레이어 탐지 범위
    // 찾은 타겟
    public Transform curTarget{get; private set;}
    // 타겟까지의 거리.
    public float distanceToTarget{get; private set;}
    public bool isCriticalTarget{get; private set;} = false;
    
    void Start(){
        
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(FindTargetRoutine(0.4f));   
        }
    }
    

    IEnumerator FindTargetRoutine(float delay)
    {
        while (true)
        {
            // 0.4초 마다 가장 가까운 플레이어 찾기
            FindObj();
            yield return new WaitForSeconds(delay);
        }
    }
    
    
    // 원거리 공격 범위까지 가장 가까운 플레이어를 찾게됨.
    private void FindObj()
    {
        // 10f내 CriticalObj 찾기
        Collider[] cirticals = Physics.OverlapSphere(transform.position,detectCriticlaRange,
            LayerMask.GetMask("CriticalObj"));

        // 찾았다면
        if (cirticals.Length > 0)
        {
            Transform nearestCritical = null;
            float minDistanceToCri = float.MaxValue;

            foreach (Collider c in cirticals)
            {
                float distanceToCri = (transform.position - c.transform.position).sqrMagnitude;

                if (distanceToCri < minDistanceToCri)
                {
                    minDistanceToCri = distanceToCri;
                    nearestCritical = c.transform;
                }
            }
            curTarget = nearestCritical;
            distanceToTarget = Mathf.Sqrt(minDistanceToCri);
            isCriticalTarget = true;
            return;
        }
        
        // 공격 거리 기준 가상 구
        Collider[] players = Physics.OverlapSphere(transform.position, 
            detectRange, LayerMask.GetMask("Player"));
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
        
        curTarget = nearest;
        
        distanceToTarget = (curTarget != null) ? Mathf.Sqrt(minDistance) : float.MaxValue;
        // 플레이럴 찾았다는건 CriticalOBj를 못 찾았다는 것.
        isCriticalTarget = false;
    }
}
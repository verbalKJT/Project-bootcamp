using System.Collections;
using Photon.Pun;
using UnityEngine;

public class EnemySensor : MonoBehaviourPun
{
    private float detectRange = 30f; // 몬스터의 플레이어 탐지 범위
    // 찾은 타겟
    public Transform curTarget{get; private set;}
    // 타겟까지의 거리.
    public float distanceToTarget{get; private set;}
    
    void Start(){
        
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(FindPlayerRangedRoutine(0.4f));   
        }
    }
    

    IEnumerator FindPlayerRangedRoutine(float delay)
    {
        while (true)
        {
            // 0.4초 마다 가장 가까운 플레이어 찾기
            FindPlayer();
            yield return new WaitForSeconds(delay);
        }
    }
    
    // 원거리 공격 범위까지 가장 가까운 플레이어를 찾게됨.
    private void FindPlayer()
    {
        // 공격 거리 기준 가상 구
        Collider[] players = Physics.OverlapSphere(transform.position,detectRange, LayerMask.GetMask("Player"));
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
    }
}
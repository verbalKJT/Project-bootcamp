using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyMove : MonoBehaviourPun
{
    [Header("이동할 위치")]
    [SerializeField] private Transform[] movePoint; // 반드시 이동할 위치들

    [SerializeField] private Transform[] wayPoint; // 갈림길 위치들
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); // 적 생성 후 바로 초기화해주기 위해 awake 권장
    }

    void Start()
    {
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
    // GameManager에서 경로를 주입받는 초기화 함수
    public void SetMovePoints(Transform[] moves, Transform[] ways)
    {
        this.movePoint = moves;
        this.wayPoint = ways;
        
        // 이동 계산은 마스터 클라이언트만
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(GotoDestination(0)); // 경로를 받은 뒤에 이동 시작
        }
    }
    private IEnumerator GotoDestination(int index)
    {
        agent.SetDestination(movePoint[index].position);
        //경로를 계산 중이거나, 남은 거리가 정지 거리보다 크면 계속 기다림
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null; 
        }
        if (index == 0) // 첫 번째 포인트(갈림길 입구)에 도착했다면
        {
            int way = Random.Range(0, wayPoint.Length); // 0 또는 1 (갈림길 개수만큼)
            yield return StartCoroutine(GotoCrossroads(way, index));
        }
        else // 갈림길 입구가아닌 갈림편 출구에 도착했다면
        {
            // 인덱스 범위 체크 추가 (안전장치)
            if (index + 1 < movePoint.Length)
            {
                agent.SetDestination(movePoint[index + 1].position); // 최종목적지
            }
        }

        if (index == movePoint.Length - 1) // 마지막 목적지로 향하고 있으면
        {
            StopAllCoroutines(); // 이동 종료
        }
    }

    private IEnumerator GotoCrossroads(int way,int wayIndex)
    {
        int index = wayPoint[way].childCount - 1; // 갈림길 자식 갯수 -1 = index
        Transform target = wayPoint[way].GetChild(index);
        Debug.Log(target.name + " " + index);
        agent.SetDestination(target.position);
        //경로를 계산 중이거나, 남은 거리가 정지 거리보다 크면 계속 기다림
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null; 
        }
        StartCoroutine(GotoDestination(wayIndex+1));
    }
}

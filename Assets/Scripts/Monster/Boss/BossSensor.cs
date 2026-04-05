using System.Collections;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class BossSensor : MonoBehaviourPun
{
    public static BossSensor Instance { get; private set; }

    // 탐지 범위 
    private float detectionRange = 100f;

    // 목표
    private CriticalObj _criticalObj; // 위치가 변하지 않아서 괜찮음
    public Transform criticalObjTransform;  // criticalObj의 위치
    public List<Transform> playerTransforms = new List<Transform>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
                Destroy(gameObject);
        }
    }

    void Start()
    {
        _criticalObj = FindObjectOfType<CriticalObj>();
        if (_criticalObj != null)
        {
            criticalObjTransform = _criticalObj.transform;
        }
        
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(FindPlayersTrasform(1f));
        }
    }

    IEnumerator FindPlayersTrasform(float waitTime)
    {
        while (true)
        {
            FindPlayersTrasform();
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void FindPlayersTrasform()
    {
        // 플레이어 찾기
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Player"));

        // 찾을 때마다 초기화
        playerTransforms.Clear();
        foreach (Collider player in players)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null && !health.isDead && !health.isDead)
            {
                playerTransforms.Add(player.transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 1. 원의 색상 설정 (반투명한 파란색)
        Gizmos.color = new Color(0, 0, 1, 0.3f);

        // 2. 속이 꽉 찬 구 그리기 (범위 확인용)
        Gizmos.DrawSphere(transform.position, detectionRange);

        // 3. 테두리 선 그리기 (경계선 확인용)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    [Header("적 스폰 위치")]
    public Transform[] enemySpawnPoints;
    [Header("적 이동 경로")]
    public Transform[] MovePoints; 
    public Transform[] WayPoints; 
    
    [Header("적 프리팹 이름 리스트")]
    public string[] enemyPrefabNames = { "ForestMon1" };
    
    // 생성된 몬스터 들이 들어갈 리시트
    public static List<EnemyHealth> enemies = new List<EnemyHealth>();
    
    // 생성 중인지
    private bool isSpawning = false;
    
    // 적 생성 로직
    void SpawnEnemies()
    {
        if (!PhotonNetwork.IsMasterClient) return; // 호스트만 실행

        // 몬스터가 추가되면 무리로 나옴.
        for (int i = 0; i < enemyPrefabNames.Length && i < enemySpawnPoints.Length; i++)
        {
            string enemyName = enemyPrefabNames[i];
            Transform spawnPoint = enemySpawnPoints[i];
            string path = "Enemies/" + enemyName;
                
            // 적 생성 // RoomObject로 생성자가 나가도 남아있도록
            GameObject enemyObj = PhotonNetwork.InstantiateRoomObject(path, spawnPoint.position, Quaternion.identity);
            // 적의 EnemyMove 컴포넌트 가져오기
            EnemyMove enemyMove = enemyObj.GetComponent<EnemyMove>();
            // 경로 정보 주입
            if (enemyMove != null)
            {
                enemyMove.SetMovePoints(MovePoints, WayPoints); 
            }
        }
    }
    void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        // 설정한 수 만큼 보다 살아있는 개체가 적다면 
        if (CheckMonCnt() && !isSpawning)
        {
            isSpawning = true;
            // 2~4초간
            float random = Random.Range(2f, 4f);
            // 적 생성 
            StartCoroutine(RandomSpawnEnemies(random));
            SpawnEnemies();
        }
    }
    
    private bool CheckMonCnt()
    {
        // 마스터 클라이언트만 세기 
        if (!PhotonNetwork.IsMasterClient) return false;
        
        // 50마리 정도 생각 중
        // 살아있는 몬스터의 수
        return (enemies.Count <1) ?  true : false;
    }

    IEnumerator RandomSpawnEnemies(float time)
    {
        yield return new WaitForSeconds(time);
        
        SpawnEnemies();
        isSpawning = false;
    }
}

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("스폰 위치들")]
    public Transform[] playerSpawnPoints;
    public Transform[] enemySpawnPoints;

    [Header("적 이동 경로")]
    public Transform[] MovePoints; 
    public Transform[] WayPoints;  
    
    [Header("적 프리팹 이름 리스트")]
    public string[] enemyPrefabNames = { "ForestMon1" };

    private const string SELECTED_CHAR = "SelectedChar";
        
    // 죽은 몬스터의 수
    public static int DeadMonCnt = 0;
    
    // 보스 생성 후 타임라인 시네마틱 플래그
    public static bool isCinematic = false;
    
    IEnumerator Start()
    {
        yield return null; //  1프레임 대기

        SpawnPlayerCharacter();
        //SpawnEnemies();
    }

    // 플레이어 생성 로직
    void SpawnPlayerCharacter()
    {
        // 로컬 플레이어의 커스텀 프로퍼티에서 선택된 캐릭터 인덱스 가져오기
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(SELECTED_CHAR, out object selectedCharObj))
        {
            int charIndex = (int)selectedCharObj;

            string prefabName = GetCharacterPrefabName(charIndex);
            if (string.IsNullOrEmpty(prefabName))
            {
                Debug.LogError("캐릭터 프리팹 이름이 잘못됨!");
                return;
            }
            
            // Resources 폴더에서 로드 확인
            GameObject prefab = Resources.Load<GameObject>("Heroes/" + prefabName);
            if (prefab == null)
            {
                Debug.LogError($"프리팹 로드 실패: Heroes/{prefabName}");
                return;
            }
            
            // ActorNumber를 활용해 스폰 포인트 순환 배정
            int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber % playerSpawnPoints.Length;
            Vector3 spawnPos = playerSpawnPoints[spawnIndex].position;
            
            // 네트워크 상에 오브젝트 생성
            PhotonNetwork.Instantiate("Heroes/" + prefabName, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("SelectedChar 프로퍼티 없음! 기본 캐릭터 사용");
        }
    }

    // 캐릭터 인덱스를 프리팹 이름으로 변환
    string GetCharacterPrefabName(int index)
    {
        switch (index)
        {
            case 0: return "Hero_Fire";
            case 1: return "Hero_Rock";
            case 2: return "Hero_Nature";
            default: return null;
        }
    }
    
    // 적 생성 로직
    void SpawnEnemies()
    {
        if (!PhotonNetwork.IsMasterClient) return; // 호스트만 실행

        for (int i = 0; i < enemyPrefabNames.Length && i < enemySpawnPoints.Length; i++)
        {
            string enemyName = enemyPrefabNames[i];
            Transform spawnPoint = enemySpawnPoints[i];

            GameObject enemyPrefab = Resources.Load<GameObject>("Enemies/" + enemyName);
            if (enemyPrefab == null)
            {
                Debug.LogError("적 프리팹 로드 실패: " + enemyName);
                continue;
            }
                
            // 적 생성
            GameObject enemyObj = PhotonNetwork.Instantiate("Enemies/" + enemyName, spawnPoint.position, Quaternion.identity);
            // 적의 EnemyMove 컴포넌트 가져오기
            EnemyMove enemyMove = enemyObj.GetComponent<EnemyMove>();
            // 경로 정보 주입
            if (enemyMove != null)
            {
                enemyMove.SetMovePoints(MovePoints, WayPoints); 
            }
        }
    }
}

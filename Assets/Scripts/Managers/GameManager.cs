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

    [Header("적 프리팹 이름 리스트")]
    public string[] enemyPrefabNames = { "ForestMon1" };

    private const string SELECTED_CHAR = "SelectedChar";

    IEnumerator Start()
    {
        yield return null; //  1프레임 대기

        SpawnPlayerCharacter();
        SpawnEnemies();
    }

    void SpawnPlayerCharacter()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(SELECTED_CHAR, out object selectedCharObj))
        {
            int charIndex = (int)selectedCharObj;

            string prefabName = GetCharacterPrefabName(charIndex);
            if (string.IsNullOrEmpty(prefabName))
            {
                Debug.LogError("캐릭터 프리팹 이름이 잘못됨!");
                return;
            }

            GameObject prefab = Resources.Load<GameObject>("Heroes/" + prefabName);
            if (prefab == null)
            {
                Debug.LogError($"프리팹 로드 실패: Heroes/{prefabName}");
                return;
            }

            int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber % playerSpawnPoints.Length;
            Vector3 spawnPos = playerSpawnPoints[spawnIndex].position;

            PhotonNetwork.Instantiate("Heroes/" + prefabName, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("SelectedChar 프로퍼티 없음! 기본 캐릭터 사용");
        }
    }

    string GetCharacterPrefabName(int index)
    {
        switch (index)
        {
            case 0: return "Hero_Fire";
            case 1: return "Hero_Stone";
            case 2: return "Hero_Grass";
            default: return null;
        }
    }

    void SpawnEnemies()
    {
        if (!PhotonNetwork.IsMasterClient) return;

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

            PhotonNetwork.Instantiate("Enemies/" + enemyName, spawnPoint.position, Quaternion.identity);
        }
    }
}

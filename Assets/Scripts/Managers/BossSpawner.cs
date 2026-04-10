using System.Collections;
using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class BossSpawner : MonoBehaviourPun
{
    public static BossSpawner BossSpawnerInstance { get; private set; }

    [SerializeField] private CinemachineCamera bossCam;
    [SerializeField] private PlayableDirector director;

    [SerializeField] private GameObject spawnEffect;
    public int diedCnt { get; private set; }

    private bool isSpawned = false;

    void Awake()
    {
        // 2. 씬에 하나만 존재하도록 보장하는 초기화 로직
        if (BossSpawnerInstance == null)
        {
            BossSpawnerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncreaseDiedCnt()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        diedCnt++;
        if (diedCnt >= 1 && !isSpawned) // 30마리정도 생각중
        {
            SpawnBoss();
            isSpawned = true;
        }
    }

    private void SpawnBoss()
    {
        GameObject boss =
            PhotonNetwork.InstantiateRoomObject("Enemies/" + "Boss", transform.position, transform.rotation, 0);
        photonView.RPC("SpawnBossCinematic", RpcTarget.All);
    }

    [PunRPC]
    public void SpawnBossCinematic()
    {
        spawnEffect.SetActive(true);
        if (bossCam != null)
        {
            CinemachineBasicMultiChannelPerlin noise = bossCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null)
            {
                noise.enabled = true;
            }
                
            director.Play();
            StartCoroutine(SpawnBossCinematicRoutine((float)director.duration, noise));
            GameManager.isCinematic = true;
        }
    }

    IEnumerator SpawnBossCinematicRoutine(float duration, CinemachineBasicMultiChannelPerlin noise)
    {
        yield return new WaitForSeconds(duration);
        if (BGMManager.instance != null)
        {
            BGMManager.instance.PlayBGM("Midnight_Boss");
        }
        GameManager.isCinematic = false;
        if (noise != null)
            noise.enabled = false;
        spawnEffect.SetActive(false);
    }
}
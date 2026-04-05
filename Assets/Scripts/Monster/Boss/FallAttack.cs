using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FallAttack : MonoBehaviourPun
{
    // 플레이어의 위치는 Sensor의 List로 저장되어 있음.
    [Header("낙하 오브젝트")]
    [SerializeField] private GameObject cristal;

    // BossAttack에서 부를 시작 메소드
    public void CallFallAttack()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        int rand = Random.Range(0, 10);

        if (rand < 7 && BossSensor.Instance != null && BossSensor.Instance.playerTransforms.Count > 0)
        {
            FallCrystal(BossSensor.Instance.playerTransforms);
        }
        else
        {
            FallCrystal(BossSensor.Instance.criticalObjTransform);
        }
        
        
    }
    
    //플레이어들의 위치 또는 Obj를 공격 할텐데.
    private void FallCrystal(List<Transform> playerTransforms)
    {
        foreach (Transform player in playerTransforms)
        {
            Vector3 spawnPos = player.position + Vector3.up * 8f;
            
            PhotonNetwork.Instantiate("Enemies/" +cristal.name, spawnPos, Quaternion.identity);       
        }
    }

    private void FallCrystal(Transform obj)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 spawnPos = obj.position + Vector3.up * 15f;
            
            GameObject c = PhotonNetwork.InstantiateRoomObject("Enemies/" +cristal.name, spawnPos, Quaternion.identity);
            c.GetComponent<Rigidbody>().useGravity = false;
        }
    }
}
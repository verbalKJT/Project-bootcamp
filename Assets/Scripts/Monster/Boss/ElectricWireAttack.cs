using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ElectricWireAttack : MonoBehaviourPun
{
    [Header("Electric")] [SerializeField] private GameObject horizonElectric;
    [SerializeField] private GameObject verticalElectric;

    [Header("생성 위치")] [SerializeField] private Transform horizon;
    [SerializeField] private Transform[] vertical; // 4개정도

    private int callCnt = 8; // 전깃줄 공격횟수
    private int spawnCnt = 0; // 생성된 전깃줄 개수

    // 시작 메소드
    public void CallElectric()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(CallElectricRoutine(1f));
    }

    // 수평 긴 전깃줄 하나
    private void SpawnHorizonElectric(Vector3 direction)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        GameObject e = PhotonNetwork.InstantiateRoomObject("Enemies/" + horizonElectric.name, horizon.position,
            Quaternion.identity);
        e.GetComponent<Rigidbody>().AddForce(direction * 20f, ForceMode.VelocityChange);
    }

    // 수직 전깃줄 2~3개
    private void SpawnVerticalElectric(int dir)
    {
        int spawnCount = Random.Range(1, 5); // 생성될 수직 전깃줄의 개수

        List<int> candidates = new List<int>();
        for (int i = 0; i < vertical.Length; i++)
        {
            candidates.Add(i);
        }

        // 지정된 개수 만큼 중복 없이 뽑아서 생성
        for (int i = 0; i < spawnCount; i++)
        {
            // 후보군 중 하나
            int randIdx = Random.Range(0, candidates.Count);

            // 전깃줄이 생성될 위치의 인덱스
            int selected = candidates[randIdx];

            // 선택된 위치
            Transform spawnPoint = vertical[selected];

            if (PhotonNetwork.IsMasterClient)
            {
                GameObject e = PhotonNetwork.InstantiateRoomObject("Enemies/" + verticalElectric.name
                    , spawnPoint.position, verticalElectric.transform.localRotation);
                e.GetComponent<Rigidbody>().AddForce(dir * e.transform.forward * 30f, ForceMode.Impulse);
            }

            candidates.RemoveAt(randIdx); // 생성했던 위치 후보군 제외
            // 중복 없이
        }
    }

    private IEnumerator CallElectricRoutine(float delay)
    {
        for (int i = 0; i < callCnt; i++)
        {
            spawnCnt++;
            int rand = Random.Range(0, 2);

            int temp = spawnCnt % 2;

            if (temp == 0)
            {
                if (rand == 0)
                {
                    SpawnHorizonElectric(horizon.forward);
                }
                else
                {
                    SpawnVerticalElectric(1);
                }
            }
            else
            {
                if (rand == 0)
                {
                    SpawnHorizonElectric(-horizon.forward);
                }
                else
                {
                    SpawnVerticalElectric(-1);
                }
            }

            yield return new WaitForSeconds(delay);
        }

        // 초기화 
        spawnCnt = 0;
    }
}
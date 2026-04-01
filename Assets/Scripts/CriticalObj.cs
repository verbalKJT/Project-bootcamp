using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class CriticalObj : MonoBehaviourPun, IPunObservable
{
    public int maxhp { get; private set; } = 1000;
    public int curHp { get; private set; }

    // 체력이 변할 때마다 호출될 이벤트 (현재, 최대)체력
    public event Action <int,int> onHpChange;
    
    private ReflectObj _reflectObj;
    
    void Start()
    {
        curHp = maxhp;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curHp);
        }
        else
        {
            int receiveHp = (int)stream.ReceiveNext();

            curHp = receiveHp;
            // UI 갱신
            onHpChange?.Invoke(curHp, maxhp);
        }
    }

    public void TakeDamageObj(int damage)
    {
        if(!PhotonNetwork.IsMasterClient) return;
        
        curHp -= damage;
        // 이벤트 발생
        onHpChange?.Invoke(curHp, maxhp);
    }

    [PunRPC]
    public void DestroyObj()
    {
        if(!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(GameOver(3f));
        // 폭팔 효과
    }

    IEnumerator GameOver(float time)
    {
        // 게임 종료 전 할일
        
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(gameObject);
    }
}
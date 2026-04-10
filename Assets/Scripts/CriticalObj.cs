using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class CriticalObj : MonoBehaviourPunCallbacks, IPunObservable
{
    public int maxhp { get; private set; } = 1400;
    public int curHp { get; private set; }

    // 체력이 변할 때마다 호출될 이벤트 (현재, 최대)체력
    public event Action <int,int> onHpChange;
    
    private ReflectObj _reflectObj;
    
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private string overSceneName = "Over";

    public static bool isDestroyed { get; private set; } = false;
    
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
        if (isDestroyed) return;
        
        curHp = Mathf.Max(curHp - damage, 0);
        // 이벤트 발생
        onHpChange?.Invoke(curHp, maxhp);

        if (curHp == 0)
        {
            photonView.RPC(nameof(DestroyObj), RpcTarget.All);
        }
    }

    [PunRPC]
    public void DestroyObj()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        StartCoroutine(GameOver(3f));
    }

    IEnumerator GameOver(float time)
    {
        yield return new WaitForSeconds(time);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(overSceneName);
        }
    }
}

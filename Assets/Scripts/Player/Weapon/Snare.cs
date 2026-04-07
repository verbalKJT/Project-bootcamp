using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Snare : MonoBehaviourPun
{
    [SerializeField] private Weapon weaponData;

    private EnemyHealth target;

    private GameObject master;

    [Header("피격 시 효과, 오디오")] [SerializeField]
    private GameObject hitEffect;

    void Start()
    {
        // 10초 후 삭제
        StartCoroutine(DestroySelf(10f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        Vector3 hitPos = other.ClosestPoint(transform.position);
        if (other.tag == "Monster")
        {
            if (!photonView.IsMine) return;
            // OnTriggerEnter 호출이 Update에서 자기 자신 가능하게 바꿔 놓음
            target = other.GetComponent<EnemyHealth>();
            // 모두에게 스턴 애니메이션 출력
            target.photonView.RPC("Is_Stun", RpcTarget.All, true, weaponData.effect.effectTime);
            // 데미치 처리
            target.TakeDamage(weaponData.damage); // 공격
            photonView.RPC("SnareHitEffect", RpcTarget.All, hitPos);
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            LivingEnitiy target = other.GetComponent<BossHp>();
            target.TakeDamage(weaponData.damage);
            photonView.RPC("SnareHitEffect", RpcTarget.All, hitPos);
            if (photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
            
        }
    }
    

    void Update()
    {
        if (!photonView.IsMine) return;

        // 플레이어와의 일정거리 이상 떨어지면 파괴 -> 사정거리 
        if (!DistanceToMaster() && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void SetMaster(GameObject master)
    {
        this.master = master;
    }

    private bool DistanceToMaster()
    {
        // 플레이어와 Snare 자신과의 거리 
        float distance = Vector3.Distance(transform.position, master.transform.position);


        return distance <= 45f;
    }

    IEnumerator DestroySelf(float time)
    {
        yield return new WaitForSeconds(time);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void SnareHitEffect(Vector3 hitPos)
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, hitPos, Quaternion.identity);
        }
    }
}
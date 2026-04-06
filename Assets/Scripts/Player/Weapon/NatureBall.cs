using System.Collections;
using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class NatureBall : MonoBehaviourPun
{
    // Damage
    [Header("WeaponData")] public Weapon weaponData;
    [Header("피격 효과")] [SerializeField] private GameObject hitEffect;

    void Start()
    {
        if (!photonView.IsMine) return;
        StartCoroutine(DestroySelf(6f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!photonView.IsMine) return;
        Vector3 hitPos = other.ClosestPoint(transform.position);
        if (other.tag == "Monster")
        {
            EnemyHealth target = other.GetComponent<EnemyHealth>();
            //hitEffect = PhotonNetwork.Instantiate(hitEffect.name, transform.position, transform.rotation);
            target.photonView.RPC("Is_Hit", RpcTarget.All);

            target.TakeDamage(weaponData.damage);

            //몬스터한테 맞았으면 삭제 // 네트워크 오브젝트 삭제는 서버 역할
            if (PhotonNetwork.IsMasterClient)
            {
                //PhotonNetwork.Destroy(hitEffect);
                PhotonNetwork.Destroy(gameObject);
            }
            photonView.RPC("NatureBallHitEffect",RpcTarget.All,hitPos);
        }else if (other.gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            LivingEnitiy target = other.GetComponent<BossHp>();
            target.TakeDamage(weaponData.damage);
            photonView.RPC("NatureBallHitEffect", RpcTarget.All, hitPos);
        }    
    }

    IEnumerator DestroySelf(float duration)
    {
        yield return new WaitForSeconds(duration);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void NatureBallHitEffect(Vector3 hitPos)
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, hitPos, Quaternion.identity);
        }
    }
}
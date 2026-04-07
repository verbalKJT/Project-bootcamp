using System.Collections;
using Photon.Pun;
using UnityEngine;

public class ElectricWire : MonoBehaviourPun
{
    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffect;
    
    private int damage = 20;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DestroySelf(8f));
        }
    }
    void OnTriggerEnter(Collider other)
    {
        LayerMask collisionMask = other.gameObject.layer;
        if (collisionMask == LayerMask.NameToLayer("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                player.TakeDamage(damage);
                photonView.RPC("ElectricHit", RpcTarget.All,hitPoint);
                // 네트워크에서 생성하면 괜히 Ping만 많아짐
            }
        }
    }

    IEnumerator DestroySelf(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void ElectricHit(Vector3 hitPoint)
    {
        Instantiate(hitEffect,hitPoint
            , Quaternion.identity);
    }
}
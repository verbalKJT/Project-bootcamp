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
            player.TakeDamage(damage);
            if (PhotonNetwork.IsMasterClient)
            {
                // 네트워크에서 생성하면 괜히 Ping만 많아짐
                Instantiate(hitEffect,
                    other.ClosestPoint(transform.position), Quaternion.identity);
            }
        }
    }

    IEnumerator DestroySelf(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        PhotonNetwork.Destroy(gameObject);
    }
}
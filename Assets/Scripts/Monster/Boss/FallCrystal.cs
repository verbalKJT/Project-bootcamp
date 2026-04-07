using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FallCrystal : MonoBehaviourPun
{
    private int damage = 30;

    private Rigidbody rb;

    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private GameObject particle; // 효과

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        if (!PhotonNetwork.IsMasterClient)
        {
            rb.isKinematic = true;
            return;
        }
        Instantiate(spawnEffect, transform.position + Vector3.up, Quaternion.identity);
        rb.AddForce(Vector3.down * 10f, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        LayerMask collisionMask = collision.gameObject.layer;
        Vector3 exactHitPoint = collision.contacts[0].point;
        if (collisionMask == LayerMask.NameToLayer("Player"))
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            player.TakeDamage(damage);
        }
        else if (collisionMask == LayerMask.NameToLayer("Crystal"))
        {
            CriticalObj ctx = collision.gameObject.GetComponent<CriticalObj>();
            ctx.TakeDamageObj(damage);
        }
        
        photonView.RPC("FallHit", RpcTarget.All, exactHitPoint);
        PhotonNetwork.Destroy(this.gameObject);
        
    }
    [PunRPC]
    public void FallHit(Vector3 hitPoint)
    {
        Instantiate(particle, hitPoint, Quaternion.identity);
    }
}
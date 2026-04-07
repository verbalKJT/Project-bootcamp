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
        Instantiate(spawnEffect, transform.position + Vector3.up, Quaternion.identity);
        rb.AddForce(Vector3.down * 10f, ForceMode.VelocityChange);

        StartCoroutine(DestroySelf(2f));
    }

    void OnCollisionEnter(Collision collision)
    {
        LayerMask collisionMask = collision.gameObject.layer;

        LayerMask elseMask = LayerMask.GetMask("Ground", "Wall", "NavMesh");

        if (!PhotonNetwork.IsMasterClient) return;
        Vector3 exactHitPoint = collision.contacts[0].point;
        if (collisionMask == LayerMask.NameToLayer("Player"))
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            player.TakeDamage(damage);
            photonView.RPC("FallHit", RpcTarget.All, exactHitPoint);
        }
        else if (collisionMask == LayerMask.NameToLayer("Crystal"))
        {
            CriticalObj ctx = collision.gameObject.GetComponent<CriticalObj>();
            ctx.TakeDamageObj(damage);
            photonView.RPC("FallHit", RpcTarget.All, exactHitPoint);
        }
        else
        {
            photonView.RPC("FallHit", RpcTarget.All, exactHitPoint);
        }

        DestroyCrystal();
    }

    public void DestroyCrystal()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(this.gameObject);
    }

    IEnumerator DestroySelf(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyCrystal();
    }

    [PunRPC]
    public void FallHit(Vector3 hitPoint)
    {
        Instantiate(particle, hitPoint, Quaternion.identity);
    }
}
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FallCrystal : MonoBehaviourPun
{
    private int damage = 30;

    private Rigidbody rb;

    [SerializeField] private GameObject particle; // 효과

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        rb.AddForce(Vector3.down * 10f, ForceMode.VelocityChange);

        StartCoroutine(DestroySelf(3f));
    }

    void OnCollisionEnter(Collision collision)
    {
        LayerMask collisionMask = collision.gameObject.layer;

        LayerMask elseMask = LayerMask.GetMask("Ground", "Wall", "NavMesh");
        
        if(!PhotonNetwork.IsMasterClient) return;
        
        if (collisionMask == LayerMask.NameToLayer("Player"))
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            player.TakeDamage(damage);
            DestroyCrystal();
        }
        else if (collisionMask == LayerMask.NameToLayer("Crystal"))
        {
            CriticalObj ctx = collision.gameObject.GetComponent<CriticalObj>();
            ctx.TakeDamageObj(damage);
            DestroyCrystal();

        }else if (collisionMask == LayerMask.NameToLayer("Ground"))
        {
            // 충돌이 발생한 가장 첫번쨰 포인트
            Vector3 exactHitPoint = collision.contacts[0].point;
            Instantiate(particle, exactHitPoint, Quaternion.identity);
            DestroyCrystal();
        }
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
}
using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class NatureBall : MonoBehaviourPun
{
    // Damage
    [Header("WeaponData")] public Weapon weaponData;
    
    private Rigidbody rb;
    private SphereCollider sc;
    
    [SerializeField] private Transform firePoint;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();

        Shot();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            EnemyHealth target = other.GetComponent<EnemyHealth>();
            
            target.photonView.RPC("Is_Hit",RpcTarget.All);
            
            target.TakeDamage(weaponData.damage);
            
            //몬스터한테 맞았으면 삭제
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    private void Shot()
    {
        rb.AddForce(transform.forward * weaponData.speed);
    }
}

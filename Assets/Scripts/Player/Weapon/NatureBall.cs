using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class NatureBall : MonoBehaviourPun
{
    // Damage
    [Header("WeaponData")] public Weapon weaponData;
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
    
}

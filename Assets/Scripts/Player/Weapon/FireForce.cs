using Photon.Pun;
using UnityEngine;

public class FireForce : MonoBehaviourPun
{
    private int damage = 50; // 공격력

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            LivingEnitiy target = other.GetComponent<EnemyHealth>();
            target.photonView.RPC("Is_Hit", RpcTarget.All);
            target.TakeDamage(damage);
        }
    }
}

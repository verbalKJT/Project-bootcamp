using Photon.Pun;
using UnityEngine;

public class PlayerFeedback : MonoBehaviourPun
{
    [Header("공용 오디오")] [SerializeField] protected AudioSource audioSource;

    [Header("기본공격 시 효과 및 오디오")]
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private GameObject hitEffect;
    
    public void BasicHit(Vector3 hitPos)
    {
        if (hitEffect != null)
            Instantiate(hitEffect, hitPos, Quaternion.identity);
    }

    [PunRPC]
    public void SpawnBaiscHitEffect(Vector3 hitPos)
    {
        BasicHit(hitPos);
    }

    public void BasicHitSound()
    {
        if (hitClip !=  null)
            audioSource.PlayOneShot(hitClip);
    }
}
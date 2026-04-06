using Photon.Pun;
using UnityEngine;

public class NatureFeedback : PlayerFeedback
{
    [Header("스네어 오디오 및 효과")] [SerializeField]
    private AudioClip snareFireClip;
    [SerializeField] private GameObject snareHitEffect;
    
    [Header("늑대 소환 오디오 및 효과")]
    [SerializeField] private GameObject wolfSpwanEffect;

    
    public void SnareFireSound()
    {
        if (snareFireClip != null)
            audioSource.PlayOneShot(snareFireClip);
    }
    
    public void WolfSpwanEffect(Vector3 position)
    {
        if (wolfSpwanEffect != null)
        {
            Instantiate(wolfSpwanEffect, position, Quaternion.identity);
        }
    }
}

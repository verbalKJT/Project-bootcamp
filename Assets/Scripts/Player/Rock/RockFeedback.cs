using Photon.Pun;
using UnityEngine;

public class RockFeedback : PlayerFeedback
{
    [Header("착지 시 효과 및 오디오")]
    [SerializeField] private AudioClip landStartClip;
    [SerializeField] private AudioClip landEndClip;
    [SerializeField] private GameObject landEffect;

    public void StartLandSound()
    {
        if (landStartClip != null)
        {
            audioSource.PlayOneShot(landStartClip);
        }
    }

    public void EndLandSound()
    {
        if (landEndClip != null)
        {
            audioSource.PlayOneShot(landEndClip);
        }
    }

    public void LandEffect(Vector3 position)
    {
        if (landEffect != null)
        {
            Instantiate(landEffect, position, Quaternion.identity);
        }
    }
}

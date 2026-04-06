using Photon.Pun;
using UnityEngine;

public class WolfFeedback : PlayerFeedback
{
    [Header("늑대 오디오")]
    [SerializeField] private AudioClip wolfSpwanClip;
    [SerializeField] private AudioClip wolfBarkClip;
    
    public void WolfSpwanSound()
    {
        if (wolfSpwanClip != null)
            audioSource.PlayOneShot(wolfSpwanClip);
    }
    
    public void WolfBarkSound()
    {
        if (wolfBarkClip != null)
            audioSource.PlayOneShot(wolfBarkClip);
    }
}
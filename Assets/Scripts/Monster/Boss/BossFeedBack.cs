using Photon.Pun;
using UnityEngine;

public class BossFeedBack : MonoBehaviourPun
{
    [Header("AudioSource")]
    [SerializeField] private AudioSource audioSource;
    
    [Header("등장 오디오")]
    [SerializeField] private AudioClip bossSpawnClip;   
    
    [Header("얼음 낙하 시작 오디오")]
    [SerializeField] private AudioClip bossFallClip;
    
    [Header("전깃줄 시작 오디오")]
    [SerializeField] private AudioClip bossElecClip;
    
    [Header("보스 사망 오디오 및 효과")]
    [SerializeField] private AudioClip bossDieClip;
    [SerializeField] private GameObject bossDieEffect;
    
    public void CallSpawnClip()
    {
        if(bossSpawnClip != null)
            audioSource.PlayOneShot(bossSpawnClip);
    }

    public void CallFallClip()
    {
        if(bossFallClip != null)
            audioSource.PlayOneShot(bossFallClip);
    }

    public void CallElecClip()
    {
        if(bossElecClip != null)
            audioSource.PlayOneShot(bossElecClip);
    }

    public void CallDieClipAndEffect()
    {
        if(bossDieClip != null)
            audioSource.PlayOneShot(bossDieClip);
        if(bossDieEffect != null)
            Instantiate(bossDieEffect, transform.position, Quaternion.identity);
    }
}

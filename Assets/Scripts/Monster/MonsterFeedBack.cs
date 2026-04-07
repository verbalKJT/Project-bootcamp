using Photon.Pun;
using UnityEngine;

public class MonsterFeedBack : MonoBehaviourPun
{
    [Header("AudioSource")] [SerializeField]
    private AudioSource audioSource;

    [Header("근접 오디오")] [SerializeField] private AudioClip nearClip;

    [Header("원거리 오디오")] [SerializeField] private AudioClip rangeClip;

    [Header("사망 오디오 및 효과")] [SerializeField] private AudioClip dieClip;
    [SerializeField] private GameObject dieEffect;
    public void CallNearClip()
    {
        if (nearClip != null)
        {
            audioSource.PlayOneShot(nearClip);
        }
    }

    public void CallRangeClip()
    {
        if (rangeClip != null)
        {
            audioSource.PlayOneShot(rangeClip);
        }
    }

    public void DieClip()
    {
        if (dieClip != null)
            audioSource.PlayOneShot(dieClip);
    }

    public void DieEffect(Vector3 pos)
    {
        Instantiate(dieEffect, pos, Quaternion.identity);
    }
}
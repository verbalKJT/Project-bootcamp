using Photon.Pun;
using UnityEngine;

public class FireFeedback : PlayerFeedback
{
   [Header("대쉬 오디오")]
   [SerializeField] private AudioClip dashClip;
   [SerializeField] private GameObject dashEffect;
   
   [Header("검기 오디오 및 피격효과")]
   [SerializeField] private AudioClip forceClip;
   [SerializeField] private GameObject forceEffect; // 아우라
   
   public void DashSound()
   {
      if(dashClip != null)
         audioSource.PlayOneShot(dashClip);
   }

   public void DashEffect(Vector3 pos, Quaternion rot)
   {
      if (dashEffect != null)
      {
         GameObject d = Instantiate(dashEffect, pos, Quaternion.identity);
         Quaternion correctedRot = rot * Quaternion.Euler(0f, 90f, 0f);
         d.transform.localRotation = correctedRot;
      }
         
   }
   public void ForceEffect() // 아우라 효과
   {
      if (forceEffect != null)
      {
         Instantiate(forceEffect, transform.position, Quaternion.identity);
      }
   }
   
   public void ForceFireSound()
   {
      if (forceClip != null)
      {
         audioSource.PlayOneShot(forceClip);
      }
   }
}

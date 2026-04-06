using System.Collections;
using Photon.Pun;
using UnityEngine;

public class TheWall : MonoBehaviourPun
{
    private float desTime = 5f;
    
    [Header("설치 시 효과")]
    [SerializeField] private GameObject setEffect;
    void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(DestroySelf(desTime));
        }
        if(setEffect != null)
            Instantiate(setEffect, transform.position, transform.rotation);
    }
    
    IEnumerator DestroySelf(float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Monster")
        {
            Rigidbody target = collision.gameObject.GetComponent<Rigidbody>();
            
            target.AddForce(-target.transform.forward * 10f, ForceMode.Impulse);
        }else if (collision.gameObject.layer == LayerMask.NameToLayer("MonsterWeapon"))
        {
            PhotonNetwork.Destroy(collision.gameObject);
        }
    }
}

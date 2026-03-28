using System.Collections;
using Photon.Pun;
using UnityEngine;

public class TheWall : MonoBehaviourPun
{
    private float desTime = 5f;
    void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(DestroySelf(desTime));
        }
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
        }
    }
}

using Photon.Pun;
using UnityEngine;

public class FallCrystal :  MonoBehaviourPun
{
    private int damage = 30;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();   
        rb.useGravity = true;
        
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided");
        }else if (other.gameObject.layer == LayerMask.NameToLayer("CriticalObj"))
        {
                
        }
    }
    
}
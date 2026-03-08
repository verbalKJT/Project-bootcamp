using Photon.Pun;
using UnityEngine;

public class Hammer : MonoBehaviourPun
{
    [SerializeField] public Weapon weaponData;
    
    private BoxCollider hammerCollider;
    void Start()
    {
        hammerCollider = GetComponent<BoxCollider>();
        // 초기 비활성화
        hammerCollider.enabled = false; 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            // OnTriggerEnter 호출이 Update에서 자기 자신 가능하게 바꿔 놓음
            LivingEnitiy target = other.GetComponent<EnemyHealth>();
            // 모두에게 맞은 애니메이션 출력
            target.photonView.RPC("Is_Hit", RpcTarget.All);
            // 데미치 처리
            target.TakeDamage(weaponData.damage); // 공격
        }
    }
    public void OnCollider()
    {
        // 공격할 때만 -> AnimEventMover로 콜라이더 키기
        hammerCollider.enabled = true;
    }

    public void OffCollider()
    {
        hammerCollider.enabled = false;
    }
}

using System.Collections;
using Photon.Pun;
using UnityEngine;

public class FireForce : MonoBehaviourPun
{
    private int damage = 50; // 공격력

    private float initalDir = 3f; // 아래로 꽂힐 초기 방향
    private float custom_Gravity = 0.1f; // 검기만 받을 중력 힘 
    private Rigidbody rb;

    [Header("피격 이펙트")] [SerializeField] private GameObject forceHitEffectPrefab;

    void Start()
    {
        StartCoroutine(DestroySelf(8f));
        rb = GetComponent<Rigidbody>();

        // 시작과 동시에 약간 위로 힘 주기
        // 생성될 때 앞으로 가는 힘은 이미 받아져있는 상태임
        rb.AddForce(Vector3.up * initalDir, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster") ||
            other.gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            LivingEnitiy target = other.GetComponentInParent<LivingEnitiy>();
            if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
                target.photonView.RPC("Is_Hit", RpcTarget.All);
            target.TakeDamage(damage);
            photonView.RPC("SpawnForceHitEffect", RpcTarget.All, hitPoint);
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject); // 네트워크에서 삭제
            }
        }
    }

    IEnumerator DestroySelf(float time)
    {
        yield return new WaitForSeconds(time);
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject); // 6초동안 아무것도 부딪히지 않으면 삭제
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        // 생성되고 고정된 시기마다 땅으로 떨어지도록 F = mg
        rb.AddForce(Vector3.down * custom_Gravity, ForceMode.Impulse);

        // 원래 날아가는 방향을 처다보도록
        if (rb.linearVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
        }
    }

    [PunRPC]
    public void SpawnForceHitEffect(Vector3 hitPoint)
    {
        if (forceHitEffectPrefab != null)
        {
            Instantiate(forceHitEffectPrefab, hitPoint, Quaternion.identity);
        }
    }
}
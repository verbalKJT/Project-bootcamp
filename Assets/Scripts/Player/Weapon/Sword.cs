using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Sword : MonoBehaviourPun
{
    [SerializeField] public Weapon weaponData;
    [SerializeField] BoxCollider swordCollider;

    [Header("검기 OBJ,발사 위치")] [SerializeField]
    private Transform forcePos;

    [Header("검 중앙")] public Transform center;
    public bool isStrike = false;
    public Vector3 prevPos;
    public HashSet<Collider> hitTargets = new HashSet<Collider>();

    private GameObject swordForce;

    private FireFeedback feedback;

    void Start()
    {
        swordCollider.enabled = false; // 콜라이더 끄기
        feedback = GetComponentInParent<FireFeedback>();
    }

    void Update()
    {
        if (!photonView.IsMine || !isStrike) return;
        CheckStrikeBound();
    }
    
    // 애니메이션 이벤트로 호출
    public void ShotForce()
    {
        if (!photonView.IsMine) return;
        // 플레이어 스킬 발사체는 소유 플레이어 기준으로 생성해야 한다.
        swordForce = PhotonNetwork.Instantiate("Heroes/Slash Projectile VFX Eletric", forcePos.position,
            forcePos.rotation);

        Rigidbody forceRb = swordForce.GetComponent<Rigidbody>();
        if (forceRb != null)
        {
            forceRb.AddForce(forcePos.forward * weaponData.speed, ForceMode.Impulse);
        }
    }

    public void CheckStrikeBound()
    {
        Vector3 curPos = center.position;

        Vector3 direction = curPos - prevPos;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            int hitMask = LayerMask.GetMask("Monster", "Boss");

            RaycastHit[] hits = Physics.SphereCastAll(prevPos, 1.2f, direction.normalized, distance, hitMask);

            foreach (RaycastHit hit in hits)
            {
                Collider hitCol = hit.collider;

                if (hitTargets.Contains(hitCol)) continue;

                hitTargets.Add(hitCol);

                LivingEnitiy target = hitCol.GetComponentInParent<LivingEnitiy>();
                if (target == null) continue;

                if (target.gameObject.layer == LayerMask.NameToLayer("Monster"))
                {
                    target.photonView.RPC("Is_Hit", RpcTarget.All);
                }

                target.TakeDamage(weaponData.damage); 
                feedback.photonView.RPC("SpawnBaiscHitEffect", RpcTarget.All,hit.point);
            }
        }

        prevPos = curPos;
    }
}

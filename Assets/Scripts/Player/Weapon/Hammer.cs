using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Hammer : MonoBehaviourPun
{
    [SerializeField] public Weapon weaponData;
    
    private BoxCollider hammerCollider;
    
    private RockFeedback feedback;
    [Header("해머 중앙")] public Transform center;
    public bool isStrike = false;
    public Vector3 prevPos;
    public HashSet<int> hitTargets = new HashSet<int>();

    void Start()
    {
        feedback = GetComponentInParent<RockFeedback>();
        hammerCollider = GetComponent<BoxCollider>();
        // 초기 비활성화
        hammerCollider.enabled = false; 
    }
    void Update()
    {
        if (!photonView.IsMine || !isStrike) return;
        CheckStrikeBound();
    }
    public void CheckStrikeBound()
    {
        Vector3 curPos = center.position;

        Vector3 direction = curPos - prevPos;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            int hitMask = LayerMask.GetMask("Monster", "Boss");

            RaycastHit[] hits = Physics.SphereCastAll(prevPos, 1.5f, direction.normalized, distance, hitMask);

            foreach (RaycastHit hit in hits)
            {
                Collider hitCol = hit.collider;

                LivingEnitiy target = hitCol.GetComponentInParent<LivingEnitiy>();
                if (target == null) continue;

                int targetID = target.photonView.ViewID;
                if (hitTargets.Contains(targetID)) continue;

                hitTargets.Add(targetID);
                
                if (target.gameObject.layer == LayerMask.NameToLayer("Monster"))
                {
                    target.photonView.RPC("Is_Hit", RpcTarget.All);
                }

                target.TakeDamage(weaponData.damage); 
                Vector3 hitPos = hitCol.ClosestPoint(curPos);
                feedback.photonView.RPC("SpawnBaiscHitEffect", RpcTarget.All,hitPos);
            }
        }

        prevPos = curPos;
    }
}

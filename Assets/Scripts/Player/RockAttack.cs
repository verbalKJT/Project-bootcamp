using Photon.Pun;
using UnityEngine;

public class RockAttack : PlayerAttack
{
    protected bool isShieldActive = false;
    protected bool shieldInput;

    private RockAnimMover ram;

    // 프로 퍼티로 가져다 쓰는건 편하게
    public float earthquakeTime { get; private set; } = 1f;
    public float earthquakeCool { get; private set; } = 1f;
    
    [Header("벽")]
    [SerializeField] private GameObject wall;
    public override float FirstSkillTime => earthquakeTime;
    public override float FirstSkillCool => earthquakeCool;
    void Start()
    {
        base.Start();
        ram = GetComponentInChildren<RockAnimMover>();
    }
    void Update()
    {
        base.Update();
        if (input)
        {
            photonView.RPC("AttackAnim", RpcTarget.All);
        }

        shieldInput = Input.GetMouseButton(1);
        // 버튼 상태가 이전과 달라졌을 때만 RPC 전송 (꾹 누르고 있을 때 매 프레임 호출 방지)
        if (shieldInput != isShieldActive)
        {
            isShieldActive = shieldInput;
            // RPC를 통해 모든 클라이언트에게 현재 상태(true/false)를 전달
            photonView.RPC("SyncShieldState", RpcTarget.All, isShieldActive);
        }
        earthquakeTime += Time.deltaTime; // 쿨타임 재기
        if (shift && pm.isGrounded && earthquakeTime >= earthquakeCool)
        {
            earthquakeTime = 0f;
            photonView.RPC("Earthquake", RpcTarget.All);
        }
    }

    [PunRPC]
    public void AttackAnim()
    {
        animator.SetTrigger("Attack");
    }

    [PunRPC]
    public void SyncShieldState(bool state)
    {
        animator.SetBool("Dif", state);
        if (state == false)
        {
            ram.OffShield();
        }
    }

    [PunRPC]
    public void Earthquake()
    {
        if (!photonView.IsMine) return;
        
        animator.SetTrigger("Earthquake");
        
    }

    public void QuakeJump()
    {
        pm.rb.AddForce(transform.forward * 50f, ForceMode.VelocityChange);
        pm.rb.AddForce(transform.up * 15f, ForceMode.VelocityChange);
    }

    public void Landing()
    {
        pm.rb.AddForce(Vector3.down * 50f, ForceMode.VelocityChange);
    }

    public void MakeAttackBound()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, 10f,LayerMask.GetMask("Monster"));
        
        foreach (Collider target in targets)
        {
            if (target != null)
            {
                LivingEnitiy t = target.GetComponent<EnemyHealth>();
                t.TakeDamage(30);
                t.photonView.RPC("Is_Hit", RpcTarget.All);
            }
        }
    }
}
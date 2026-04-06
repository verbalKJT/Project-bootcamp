using Photon.Pun;
using UnityEngine;

public class RockAttack : PlayerAttack
{
    protected bool isShieldActive = false;
    protected bool shieldInput;

    private RockAnimMover ram;

    // 프로 퍼티로 가져다 쓰는건 편하게
    public float earthquakeTime { get; private set; } = 8f;
    public float earthquakeCool { get; private set; } = 8f;

    [Header("벽")] public float wallTime { get; private set; } = 10f;
    public float wallCool { get; private set; } = 10f;
    [SerializeField] private GameObject wall;
    public override float FirstSkillTime => earthquakeTime;
    public override float FirstSkillCool => earthquakeCool;

    public override float SecSkillTime => wallTime;
    public override float SecSkillCool => wallCool;

    private Shield _shield;

    private int randDam = 40;
    void Start()
    {
        base.Start();
        ram = GetComponentInChildren<RockAnimMover>();
        _shield = GetComponentInChildren<Shield>();
    }

    void Update()
    {
        base.Update();
        if (!photonView.IsMine) return;
        if(GameManager.isCinematic) return;
        if (input)
        {
            photonView.RPC("AttackAnim", RpcTarget.All);
        }

        shieldInput = Input.GetMouseButton(1);
        bool shieldState = shieldInput && _shield.CanRaise;
        
        if (shieldState != isShieldActive) // 쉴드가 파괴 되지 않았을 때만
        {
            isShieldActive = shieldState;
            if (isShieldActive)
            {
                _shield.TryRaised(); // 키기
            }
            else
            {
                _shield.Lower(); // 끄기
            }

            // 상태에 맞는 애니메이션
            photonView.RPC("SyncShieldState", RpcTarget.All, isShieldActive);
        }

        earthquakeTime += Time.deltaTime; // 쿨타임 재기
        if (shift && pm.isGrounded && earthquakeTime >= earthquakeCool)
        {
            earthquakeTime = 0f;
            photonView.RPC("Earthquake", RpcTarget.All, true);
        }

        // 쿨타임
        wallTime += Time.deltaTime;
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
    public void Earthquake(bool state)
    {
        if (!photonView.IsMine) return;

        animator.SetBool("Earthquake", state);
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
        LayerMask enemyLayer = LayerMask.GetMask("Monster","Boss");
        Collider[] targets = Physics.OverlapSphere(transform.position, 15f, enemyLayer);

        foreach (Collider target in targets)
        {
            if (target != null)
            {
                if(target.gameObject.layer == LayerMask.NameToLayer("Boss"))
                {
                    LivingEnitiy b = target.gameObject.GetComponent<BossHp>();
                    b.TakeDamage(randDam);
                }else if(target.gameObject.layer == LayerMask.NameToLayer("Monster"))
                {
                    LivingEnitiy t = target.GetComponent<EnemyHealth>();
                    t.TakeDamage(randDam);
                    t.photonView.RPC("Is_Hit", RpcTarget.All);
                }
             
            }
        }
    }

    [PunRPC]
    public void SummonWall(bool state)
    {
        animator.SetBool("Wall", state);
    }

    public void SetWallTime(float time)
    {
        wallTime = time;
    }

    public void ForceShieldDown()
    {
        if (!isShieldActive)
        {
            return;
        }

        isShieldActive = false;

        if (photonView.IsMine && _shield != null)
        {
            _shield.Lower();
        }

        photonView.RPC("SyncShieldState", RpcTarget.All, false);
    }
}
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class NatureManAttack : PlayerAttack
{
    [Header("발사 위치")] [SerializeField] private Transform firePoint;

    [Header("발사체")] [SerializeField] private GameObject cannon;
    private string cannonName;
    private float cannonCoolTime;
    private float reloadTime = 0f;

    [Header("WeaponData")] [SerializeField]
    private Weapon weapon;

    [Header("소환 위치")]
    // 늑대 소환 
    private float summonTime = 20f;
    private const float summonCoolTime = 20f; // 15~20초 생각중
    [SerializeField] public Transform[] summonPoint;
    [SerializeField] private string wolfName;
    
    [Header("Snare")]
    private float snareTime = 10f;
    private const float snareCoolTime = 10f;
    [SerializeField] private GameObject snareObj;
    [SerializeField] private Transform snareSPoint; // 덩쿨 생성 위치

    public override float FirstSkillTime => snareTime;
    public override float FirstSkillCool => snareCoolTime;
    public override float SecSkillTime => summonTime;
    public override float SecSkillCool => summonCoolTime;
    
    private NatureFeedback feedback;
    void Start()
    {
        base.Start();
        cannonName = cannon.name;
        cannonCoolTime = cannon.GetComponent<NatureBall>().weaponData.reloadTime;
        feedback = gameObject.GetComponent<NatureFeedback>();
    }


    void Update()
    {
        // 부모 Update에서 IsMine을 체크하기 때문에 한번 더 체크할 필요 없음.
        base.Update();
        if(GameManager.isCinematic) return;
        // 공속 
        reloadTime += Time.deltaTime;
        if (input)
        {
            if (reloadTime >= cannonCoolTime)
            {
                photonView.RPC("AttackAnim", RpcTarget.All);
                reloadTime = 0f;
            }
        }

        summonTime += Time.deltaTime;
        if (commandE && summonTime >= summonCoolTime) // e키로 소환
        {
            photonView.RPC("SummonWolf", RpcTarget.All, true);
            summonTime = 0f; // 초기화
        }
        
        snareTime += Time.deltaTime;
        if (commandQ && snareTime >= snareCoolTime && pm.isGrounded)
        {
            photonView.RPC("Snare", RpcTarget.All);
            snareTime = 0f;
        }
    }

    [PunRPC]
    protected void AttackAnim()
    {
        animator.SetTrigger("Attack");
    }

    public void Fire()
    {
        if (photonView.IsMine)
        {
            // 생성 후 밣사
            cannon = PhotonNetwork.Instantiate("Heroes/" + cannonName, firePoint.position, Quaternion.identity);
            firePoint.forward = transform.forward; // 공격 시 총구 앞을 플레이어 캐릭터 앞으로
            cannon.GetComponent<Rigidbody>().AddForce(firePoint.forward * weapon.speed);
        }
    }

    [PunRPC]
    protected void SummonWolf(bool check)
    {
        animator.SetBool("Summon", check);
        // 소환 타이밍은 애니메이션 이벤트로
    }

    // 애니메이션 이벤트로 호출할 소환 메소드
    public void Summon()
    {
        if (!photonView.IsMine) return;
        foreach (Transform t in summonPoint)
        {
            // 소환 위치에 늑대 2개 소환
            GameObject wolfs = PhotonNetwork.Instantiate("Heroes/" + wolfName, t.position, Quaternion.identity);
            feedback.WolfSpwanEffect(t.position);
            wolfs.GetComponent<Wolf>().SetSlot(t);
        }
    }

    [PunRPC]
    protected void Snare()
    {
        animator.SetTrigger("Snare");
        animator.SetBool("Summon", false); // 해줘야 Snare -> wolf로 안넘어감 제약조건
        // 소환 타이밍은 애니메이션 이벤트로
    }

    // 애니메이션 이벤트에서 호출
    public void SnareAttack()
    {
        if (!photonView.IsMine) return;
        
        GameObject obj = PhotonNetwork.Instantiate("Heroes/"+snareObj.name, snareSPoint.position, Quaternion.identity);
        
        // 바닥을 기어야하니.
        obj.GetComponent<Rigidbody>().AddForce(snareSPoint.forward * 20f, ForceMode.Impulse);
        
        // 마스터 등록
        obj.GetComponent<Snare>().SetMaster(gameObject);
    }
}
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
    private float summonTime = 0f;

    private float summonCoolTime = 2f; // 15~20초 생각중
    [SerializeField] private Transform[] summonPoint;
    [SerializeField] private string wolfName;

    void Start()
    {
        base.Start();
        cannonName = cannon.name;
        cannonCoolTime = cannon.GetComponent<NatureBall>().weaponData.reloadTime;
    }


    void Update()
    {
        // 부모 Update에서 IsMine을 체크하기 때문에 한번 더 체크할 필요 없음.
        base.Update();
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

            wolfs.GetComponent<Wolf>().SetSlot(t);
        }

        photonView.RPC("SummonWolf", RpcTarget.All, false);
    }
}
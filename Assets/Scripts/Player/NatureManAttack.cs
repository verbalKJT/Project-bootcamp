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
        reloadTime += Time.deltaTime;
        if (input)
        {
            if (reloadTime >= cannonCoolTime)
            {
                photonView.RPC("AttackAnim", RpcTarget.All);
                reloadTime = 0f;
            }
        }
    }

    [PunRPC]
    protected void AttackAnim()
    {
        animator.SetTrigger("Attack");
        //StartCoroutine(Fire(0.7f));
    }
/*
    IEnumerator Fire(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (photonView.IsMine)
        {
            PhotonNetwork.Instantiate("Heroes/" + cannonName, firePoint.position, Quaternion.identity);
        }
    }
    */
    public void Fire()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Instantiate("Heroes/" + cannonName, firePoint.position, Quaternion.identity);
        }
    }
}
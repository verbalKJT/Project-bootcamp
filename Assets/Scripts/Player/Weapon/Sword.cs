using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] public Weapon weaponData;
    [SerializeField] BoxCollider swordCollider;

    void Start()
    {
        swordCollider.enabled = false; // 콜라이더 끄기
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            // OnTriggerEnter 호출이 Update에서 자기 자신 가능하게 바꿔 놓음
            LivingEnitiy target = other.GetComponent<EnemyHealth>();
            Debug.Log(target);
            // 모두에게 맞은 애니메이션 출력
            target.photonView.RPC("Is_Hit", RpcTarget.All);
            // 데미치 처리
            target.TakeDamage(weaponData.damage); // 공격
        }
    }

    public IEnumerator IncreaseSword(float waittime)
    {
        yield return new WaitForSeconds(waittime);
        swordCollider.enabled = true;
        Vector3 orininSize = swordCollider.size; // 기본사이즈
        Vector3 increaseSize = new Vector3(orininSize.x, 2f, orininSize.z);
        swordCollider.size = increaseSize;

        yield return new WaitForSeconds(0.5f);

        swordCollider.size = orininSize;
        swordCollider.enabled = false; // 공격할 때만 콜라이더 사용
    }
}

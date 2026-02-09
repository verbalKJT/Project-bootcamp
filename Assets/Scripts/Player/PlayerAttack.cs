using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerAttack : MonoBehaviourPun
{
    [Header("무기 콜라이더")] [SerializeField] private BoxCollider weapon_collider;

    [SerializeField] private Weapon sword;
    
    private Animator animator;
    private bool input;

    private float cooltime = 1f; // 공격 후 쉬는 시간
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        weapon_collider.enabled = false; // 콜라이더 끄기
    }

    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine) return;
        
        input = Input.GetMouseButtonDown(0);
        if (input)
        {
            StartCoroutine(IncreaseSword(1f));
            animator.SetTrigger("Attack");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            
            // OnTriggerEnter 호출이 Update에서 자기 자신 가능하게 바꿔 놓음
            LivingEnitiy target = other.GetComponent<EnemyHealth>();
            Debug.Log(target);
            // 모두에게 맞은 애니메이션 출력
            target.photonView.RPC("Is_Hit",RpcTarget.All);
            // 데미치 처리
            target.TakeDamage(sword.damage); // 공격
          
          
        }
    }
    IEnumerator IncreaseSword(float waittime)
    {
        yield return new WaitForSeconds(waittime);
        weapon_collider.enabled = true;
        Vector3 orininSize = weapon_collider.size; // 기본사이즈
        Vector3 increaseSize = new Vector3(orininSize.x, 2f, orininSize.z);
        weapon_collider.size = increaseSize;
        
        yield return new WaitForSeconds(0.5f);
        
        weapon_collider.size = orininSize;
        weapon_collider.enabled = false; // 공격할 때만 콜라이더 사용
    }
}

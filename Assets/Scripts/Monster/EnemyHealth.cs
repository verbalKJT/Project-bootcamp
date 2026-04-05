using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : LivingEnitiy
{
    [Header("적 상태")]
    [field:  SerializeField]
    public MonsterState foreast_state { get; private set; }
    
    [Header("체력 바 UI")]
    [SerializeField] private Canvas hpCanvas;
    [SerializeField] private Image hpBar;
    
    [Header("적 컴포넌트")]
    public Animator animator;

    public EnemyMove em;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       curhp = foreast_state.hp; // 체력 초기화
       hpBar.fillAmount = curhp;
       em = GetComponent<EnemyMove>();
       
       // 생성 후 스포너에 자기 자신 추가
       EnemySpawner.enemies.Add(this); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    protected override void OnHpChanged() // 자식들이 쓸 껍데기
    {
       hpBar.fillAmount = (float)curhp / foreast_state.hp;
       if (hpBar.fillAmount <= 0.3f) 
       {
           hpBar.color = Color.darkRed;
       }
       else
       {
           hpBar.color = Color.white;
       }
    }

    protected override void OnDeath() // 죽었을 때
    {
        // RPC TakeDamToMonster에서 IsDead를 체크하고 들어온 상태
        photonView.RPC("CallDieAnim",RpcTarget.All);
        
        // 사망 시 
        if (EnemySpawner.enemies.Contains(this))
        {
            EnemySpawner.enemies.Remove(this);
            // 죽은 몬스터 수 세기
            BossSpawner.BossSpawnerInstance.IncreaseDiedCnt();
        }
    }
    protected override void OnRespawn() // 다시 살아날때
    { }
    [PunRPC]
    public void Is_Hit()
    {
        animator.SetTrigger("IsHit");
    }

    [PunRPC]
    public void CallDieAnim()
    {
        // 애니메이션 변경 후
        animator.SetTrigger("IsDead");
        // agent 정지
        
        em.agent.Stop();
        StartCoroutine(DestroySelf(2f));
    }

    //  애니메이션 이벤트
    public void DestroyObj()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    IEnumerator DestroySelf(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void Is_Stun(bool isStun)
    {
        animator.SetBool("Stuned", isStun);
    }
}

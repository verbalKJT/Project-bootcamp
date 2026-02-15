using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : LivingEnitiy
{
    [Header("적 상태")]
    [SerializeField] private MonsterState foreast_state;
    
    [Header("체력 바 UI")]
    [SerializeField] private Canvas hpCanvas;
    [SerializeField] private Image hpBar;
    
    [Header("적 컴포넌트")]
    public Animator animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       curhp = foreast_state.hp; // 체력 초기화
       hpBar.fillAmount = curhp;
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
        EnemyMove em = GetComponent<EnemyMove>();
        em.agent.Stop();
    }

    public void DestroyObj()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

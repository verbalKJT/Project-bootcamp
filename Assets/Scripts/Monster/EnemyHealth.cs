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
    { }
    protected override void OnRespawn() // 다시 살아날때
    { }
    [PunRPC]
    public void Is_Hit()
    {
        animator.SetTrigger("IsHit");
    }
    
}

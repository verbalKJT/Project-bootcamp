using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : LivingEnitiy
{
    [Header("적 상태")]
    [SerializeField] private MonsterState foreast_state;
    
    [SerializeField] private Canvas hpCanvas;
    [SerializeField] private Image hpBar;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       curhp = foreast_state.hp; // 체력 초기화
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnHpChanged() // 자식들이 쓸 껍데기
    {
       hpBar.fillAmount = curhp / foreast_state.hp;
       if (hpBar.fillAmount <= 0.3f) 
       {
           hpBar.color = Color.darkRed;
       }
       else
       {
           hpBar.color = Color.white;
       }
    }
    protected virtual void OnDeath() // 죽었을 때
    { }
    protected virtual void OnRespawn() // 다시 살아날때
    { }
}

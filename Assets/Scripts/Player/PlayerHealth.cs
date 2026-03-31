using System.Collections;
using MagicPigGames; // 에셋
using Photon.Pun;
using UnityEngine; 


public class PlayerHealth : LivingEnitiy
{
    [Header("플레이어 데이터")] [SerializeField] protected PlayerState playerState;
    
    [Header("체력 바 UI")]
    [SerializeField] private ProgressBar hpBar;

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    
    void Start()
    {
        curhp =  playerState.hp;
        float ratio = (float)curhp / playerState.hp;
        hpBar.SetProgress(ratio);
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        
    }

    protected override void OnHpChanged() // 자식들이 쓸 껍데기
    {
        // 음수가 안나오도록
        float ratio = Mathf.Clamp01((float)curhp / playerState.hp);
        hpBar.SetProgress(ratio);
    }

    protected override void OnDeath() // 죽었을 때
    {
        photonView.RPC("Is_Stun", RpcTarget.All, true);
    }

    [PunRPC]
    public void Is_Stun(bool isStun)
    {
        playerMovement.animator.SetBool("Stun", isStun);
        playerMovement.canMove = !isStun;
        playerAttack.enabled = !isStun;
        if(photonView.IsMine && isStun)
            StartCoroutine(StunCoroutine(8f));
    }

    IEnumerator StunCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        curhp = playerState.hp / 2; // 절반만 다시 채우기
        // hp 변경 적용
        OnHpChanged();
        photonView.RPC("Is_Stun",RpcTarget.All,false);
        // 초기화 
        isDead = false;
    }
}

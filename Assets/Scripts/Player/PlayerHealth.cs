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
    
    private float lastDamagedTime;
    private int previousHp;
    private const float healDelay = 8f;
    private const float healInterval = 5f;
    private const int healAmount = 6;
    
    void Start()
    {
        curhp =  playerState.hp;
        float ratio = (float)curhp / playerState.hp;
        hpBar.SetProgress(ratio);
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();

        previousHp = curhp;
        if (photonView.IsMine)
        {
            lastDamagedTime = Time.time;
            StartCoroutine(HealSelf());
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (curhp < previousHp)
            {
                lastDamagedTime = Time.time;
            }
            previousHp = curhp;
            float ratio = (float)curhp / playerState.hp;
            hpBar.SetProgress(ratio);
        }
    }

    protected override void OnHpChanged() // 자식들이 쓸 껍데기
    {
        // 음수가 안나오도록
        float ratio = Mathf.Clamp01((float)curhp / playerState.hp);
        hpBar.SetProgress(ratio);
    }

    protected override void OnDeath() // 죽었을 때
    {
        if (!photonView.IsMine) return;
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

    IEnumerator HealSelf()
    {
        while (true)
        {
            yield return new WaitForSeconds(healInterval);
            
            if (!photonView.IsMine) continue;
            if(isDead) continue;
            if(curhp>= playerState.hp) continue;
            if(Time.time - lastDamagedTime < healDelay) continue;
            
            curhp = Mathf.Min(curhp + healAmount, playerState.hp);
            OnHpChanged();
        }
    }
}

using System;
using System.Collections;
using MagicPigGames;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class BossHp : LivingEnitiy
{
    [Header("컴포넌트 & 데이터")]
    [SerializeField] private Animator _animator;

    [field: SerializeField] public MonsterState bossState { get; private set; }

    [Header("보스 체력 바")]
    [SerializeField] private ProgressBar hpBar;

    private BossAttack _bossAttack;
    private BossFeedBack feedBack;
    // 게임 클리어 플래그
    public static bool BossDead =false;
    
    private void Start()
    {
        _bossAttack = GetComponent<BossAttack>();
        feedBack = GetComponent<BossFeedBack>();
        curhp = bossState.hp;
        float ratio = Mathf.Clamp01((float)curhp / bossState.hp);
        hpBar.SetProgress(ratio);
        
        feedBack.CallSpawnClip(); // 등장 오디오
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("BossSpwand", RpcTarget.All);
        }
    }

    protected override void OnHpChanged() // 자식들이 쓸 껍데기
    {
        float ratio = Mathf.Clamp01((float)curhp / bossState.hp);
        hpBar.SetProgress(ratio);
        if (_bossAttack != null && ratio <= 0.5f)
        {
            // 보스가 반피 아래다 -> 전깃줄 공격도 같이
            _bossAttack.IsHalf = true;
        }
        
        if (curhp <= 0)
        { 
            isDead = true;
        }
        
    }
    protected override void OnDeath() // 죽었을 때
    {  
        BossDead =  true;
        feedBack.CallDieClip();
        
        // RPC -> 중복일 수 있음.
        CallDieAnim();
    }
    [PunRPC]
    public void BossSpwand()
    {
        _animator.SetTrigger("Spwan");
    }
    public void CallDieAnim()
    {
        // 애니메이션 변경 후
        _animator.SetTrigger("IsDead");
        BossDead = true;    
        StartCoroutine(DestroySelf(2f));
    }
    
    IEnumerator DestroySelf(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
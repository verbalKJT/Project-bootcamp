using MagicPigGames;
using Photon.Pun;
using UnityEngine;

public class ReflectObj : MonoBehaviour
{
    private CriticalObj _criticalObj;
    [SerializeField] private ProgressBar hpBar;
    void Start()
    {
        _criticalObj = FindObjectOfType<CriticalObj>();
        if (_criticalObj != null)
        {
            // [바인딩 핵심] 대상의 체력 변화 이벤트에 내 UI 업데이트 함수를 구독시킵니다.
            _criticalObj.onHpChange += UpdateHpBar;

            // 게임 시작 직후 현재 체력으로 초기화 동기화
            UpdateHpBar(_criticalObj.curHp, _criticalObj.maxhp);
        }
    }
    
    private void UpdateHpBar(int curHp, int maxHp)
    {
        float ratio = Mathf.Clamp01((float)curHp / (float)maxHp);
        
        hpBar.SetProgress(ratio);
        if (ratio == 0)
        {
            _criticalObj.photonView.RPC("DestroyObj",RpcTarget.All);
        }
    }
}

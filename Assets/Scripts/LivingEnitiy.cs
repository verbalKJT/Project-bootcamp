using Photon.Pun;
using UnityEngine;

public class LivingEnitiy : MonoBehaviourPun, IPunObservable
{
    public int curhp; // 현재 체력
    public bool isDead = false; // 죽었는지 
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curhp);
        }
        else
        {
            int receiveHp = (int)stream.ReceiveNext(); // 바뀐 체력 얻기
            
            curhp = receiveHp; // 현제 체력 반영
            
            OnHpChanged(); // 체력바 갱신
        }
        
    }
    protected virtual void OnHpChanged() // 자식들이 쓸 껍데기
    { }
    protected virtual void OnDeath() // 죽었을 때
    { }
    protected virtual void OnRespawn() // 다시 살아날때
    { }
    
    // 몬스터가 맞았을 때
    public void TakeDamage(int damage)
    {
        photonView.RPC("TakeDamToMonster",RpcTarget.All,damage);
    }
    
    [PunRPC]
    public void TakeDamToMonster(int damage)
    {
        if (!isDead)
        {
            curhp -= damage; // 체력 빼기
            OnHpChanged(); // UI 갱신
            if (curhp <= 0)
            {
                isDead = true;
                OnDeath(); // 죽음 이벤트
            }
        }
    }
}
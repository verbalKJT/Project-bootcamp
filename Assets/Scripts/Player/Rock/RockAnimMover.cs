using Photon.Pun;
using UnityEngine;

public class RockAnimMover : MonoBehaviourPun
{
    [Header("방어")] [SerializeField] private GameObject shield;
    
    private GameObject energyShield;
    private Quaternion shieldRot;
    private Quaternion targetRot;
    
    private PlayerMovement playerMovement;
    private RockAttack rk;
    private Shield _shield;
    private RockFeedback feedback;
    private Hammer hammer;
    void Start()
    {
        shieldRot = shield.transform.localRotation;
        // shield 하위에 energyShield로 쓰일 Plane밖에 없음
        // shield 하위가 바뀌면 코드도 바껴야함. 
        energyShield = shield.gameObject.transform.GetChild(0).gameObject;
        playerMovement = GetComponentInParent<PlayerMovement>();
        rk = GetComponentInParent<RockAttack>();
        feedback = GetComponentInParent<RockFeedback>();
        _shield =  shield.GetComponent<Shield>();
        hammer = GetComponentInChildren<Hammer>();
    }
    // 애니메이션 이벤트로 위치 변경
    public void OnShield()
    {
        // 로컬 좌표 
        shield.transform.localRotation = Quaternion.Euler(-8f,-56f, 43f);
    }

    public void OffShield()
    {
        // 회전값
        shield.transform.localRotation = shieldRot;
        _shield.ApplyState();
    }

    public void OnEnergyShield()
    {
        _shield.ApplyState();
    }
    public void MakeAttackBound()
    {
        rk.MakeAttackBound();
        playerMovement.rb.linearVelocity = Vector3.zero; // 착지하는 순간 속도 0
        rk.Landing();
    }

    public void OnOffMove(int isOn)
    {
        if(!playerMovement.photonView.IsMine) return;
        bool canMove;
        if (isOn == 1)
        {
            canMove = true;
        }
        else
        {
            canMove = false;
            rk.photonView.RPC("Earthquake", RpcTarget.All,false);
        }
        // AddForce 타이밍
        playerMovement.canMove = canMove; // 회전은 가능
    }

    private void QuakeJump()
    {
        rk.QuakeJump();
    }
    public void StartAttack()
    {
        feedback.BasicShotSound(); // 기본 공격 오디오
        if(!photonView.IsMine) return;
        hammer.isStrike = true;
        hammer.hitTargets.Clear();
        hammer.prevPos = hammer.center.position;
    }

    public void EndAttack()
    {
        if(!photonView.IsMine) return;
        hammer.isStrike = false;
    }
}
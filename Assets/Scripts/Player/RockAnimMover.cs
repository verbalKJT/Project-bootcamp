using Photon.Pun;
using UnityEngine;

public class RockAnimMover : MonoBehaviour
{
    [Header("방어")] [SerializeField] private GameObject shield;
    
    private GameObject energyShield;
    private Quaternion shieldRot;
    private Quaternion targetRot;
    
    private PlayerMovement playerMovement;
    private RockAttack rk;
    
    void Start()
    {
        shieldRot = shield.transform.localRotation;
        // shield 하위에 energyShield로 쓰일 Plane밖에 없음
        // shield 하위가 바뀌면 코드도 바껴야함. 
        energyShield = shield.gameObject.transform.GetChild(0).gameObject;
        playerMovement = GetComponentInParent<PlayerMovement>();
        rk = GetComponentInParent<RockAttack>();
        

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
        energyShield.SetActive(false);
    }

    public void OnEnergyShield()
    {
        energyShield.SetActive(true);
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
        }
        // AddForce 타이밍
        playerMovement.canMove = canMove; // 회전은 가능
    }

    private void QuakeJump()
    {
        rk.QuakeJump();
    }
    
    
}
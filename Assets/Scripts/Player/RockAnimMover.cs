using UnityEngine;

public class RockAnimMover : MonoBehaviour
{
    [Header("방어")] [SerializeField] private GameObject shield;
    
    private GameObject energyShield;
    private Quaternion shieldRot;
    private Quaternion targetRot;
 
    void Start()
    {
        shieldRot = shield.transform.localRotation;
        // shield 하위에 energyShield로 쓰일 Plane밖에 없음
        // shield 하위가 바뀌면 코드도 바껴야함. 
        energyShield = shield.gameObject.transform.GetChild(0).gameObject;
    }
    // 애니메이션 이벤트로 위치 변경
    public void OnShield()
    {
        // 로컬 좌표 
        shield.transform.localRotation = Quaternion.Euler(-13f,-55f, 50f);
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
}
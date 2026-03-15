using Photon.Pun;
using UnityEngine;

public class PlayerInput : MonoBehaviourPun
{
    protected float h, v;
    protected float mouseX, mouseY; // 마우스로 회전 조
    protected float sensitivity = 30f;
    protected bool spaceBar;
    
    protected Vector3 moveV;
    protected Vector3 moveH;
    
    
    protected bool input; // 마우스 기본 공격
    protected bool commandE;
    protected bool commandQ;
    protected bool shift;
    
    protected void Update()
    {
        if (!photonView.IsMine) return;
        // ---- 이동 ----
        // 입력 받기
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        // 마우스 이동 감지
        mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        spaceBar = Input.GetKeyDown(KeyCode.Space);
        
        // ---공격 ------
        input = Input.GetMouseButtonDown(0);
        
        // 스킬 q,e로
        // 눌렀을 때 
        commandE = Input.GetKeyDown(KeyCode.E);
        commandQ = Input.GetKeyDown(KeyCode.Q);
        // 사용할지도 모름 걷기 뛰기 분리할 경우 
        shift = Input.GetKey(KeyCode.LeftShift);
    }
}

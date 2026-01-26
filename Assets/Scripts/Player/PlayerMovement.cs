using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    [Header("컴포넌트들")] [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;

    [Header("플레이어 데이터")] [SerializeField] PlayerState playerState;

    [SerializeField] Transform playerCam;

    private float h, v;
    private float mouseX, mouseY; // 마우스로 회전 조정
    private float sensitivity = 30f;
    private float spaceBar;

    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        // 입력 받기
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        // 마우스 이동 감지
        mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
/*
        spaceBar = Input.GetButton("SpaceBar") ? 0.5f : 0f; // 스페이스바가 눌렸는지

        if (spaceBar > 0f)
        {
            rb.AddForce(Vector3.up * playerState.jumpForce, ForceMode.VelocityChange);
        }
*/
        if (h != 0 || v != 0)
        {
            animator.SetFloat("Blend", 0.7f);
        }
        else
        {
            animator.SetFloat("Blend", 0f);
        }

        // 이동
        Vector3 moveV = transform.forward * v * Time.deltaTime * playerState.speed;
        Vector3 moveH = transform.right * h * Time.deltaTime * playerState.speed;
        rb.MovePosition(rb.position + moveV + moveH);

        // 회전
        Quaternion rot = Quaternion.Euler(0, mouseX * playerState.rotationSpeed, 0);
        rb.MoveRotation(rb.rotation * rot);
    }
}
using System;
using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMovement : PlayerInput
{
    // 내 캐릭터
    public static GameObject player;

    [Header("컴포넌트들")] public Rigidbody rb;
    [SerializeField] public Animator animator;
    [SerializeField] public CapsuleCollider col;
    [SerializeField] public BoxCollider boxCol;

    [Header("플레이어 시네머신 캠")] [SerializeField]
    private CinemachineCamera playerCam;

    [Header("Terrain 레이어")] [SerializeField]
    private LayerMask groundLayer;

    public bool isGrounded;

    public bool canMove = true;

    private Quaternion targetRotation;


    void Awake()
    {
        if (photonView.IsMine)
        {
            player = gameObject;

            var vcam = FindAnyObjectByType<CinemachineCamera>();

            playerCam = vcam;

            playerCam.Follow = transform;

            // 마우스 커서 중앙 고정 및 안보이게
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Start()
    {
        // 내 캐릭터가 아니면 
        if (!photonView.IsMine)
        {
            // 다른 플레이어의 캠을 끔
            if (playerCam != null)
            {
                playerCam.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        base.Update();
        if (canMove)
        {
            if (h != 0 || v != 0)
            {
                // 입력 받은 h와 v중 큰 값
                float moveValue = Mathf.Abs(h) > Mathf.Abs(v) ? h : v;


                photonView.RPC("MoveAnim", RpcTarget.All, Mathf.Abs(moveValue));

                if (moveValue == 0f)
                {
                    // 정지 상태일 때 0을 전달하여 애니메이션 멈춤
                    photonView.RPC("MoveAnim", RpcTarget.All, 0f);
                }
            }

            if (spaceBar && isGrounded) // 스페이스바를 누르면
            {
                photonView.RPC("JumpAnim", RpcTarget.All);
            }
        }

        // 회전
        Quaternion rot = Quaternion.Euler(0, mouseX * playerState.rotationSpeed, 0);
        rb.MoveRotation(rb.rotation * rot);
        // 마우스 상하 로직 필요할 듯

        isGrounded = Grounded();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return; // 점프도 내 캐릭터만

        if (rb.linearVelocity.y < 0)
        {
            // 중력 가속도 높이기 -> 떨어질 떄 팍 떨어지게
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (4f - 1) * Time.fixedDeltaTime;
        }

        if (canMove)
        {
            // 이동
            moveV = transform.forward * v * Time.fixedDeltaTime * playerState.speed;
            moveH = transform.right * h * Time.fixedDeltaTime * playerState.speed;
            rb.MovePosition(rb.position + moveV + moveH);
        }
    }

    private void Jump()
    {
        if (col != null) // 캡슐 콜라이더를 사용하는 플레이어 캐릭터
        {
            float height = col.height; // 콜라이더(캐릭터) 키 
            float jumpVelocity = Mathf.Sqrt(height * -2 * Physics.gravity.y); // v = (2gh)^2 공식사용해서 힘 구하기

            rb.AddForce(Vector3.up * jumpVelocity * 1.5f, ForceMode.VelocityChange); // 공식 활용 점프 
            // 에셋 Transform Scale이 1.5라
        }
        else // 박스 콜라이더를 사용하는 플레이어 캐릭터
        {
            float height = boxCol.size.y; // 콜라이더(캐릭터) 키 
            float jumpVelocity = Mathf.Sqrt(height * -2 * Physics.gravity.y); // v = (2gh)^2 공식사용해서 힘 구하기

            rb.AddForce(Vector3.up * jumpVelocity * 1.5f, ForceMode.VelocityChange); // 공식 활용 점프 
            // 에셋 Transform Scale이 1.5라
        }
    }

    // 바닥 확인
    private bool Grounded()
    {
        if (col != null)
        {
            bool hit;

            // 내가 지금 바닥인지
            hit = Physics.Raycast(col.bounds.center, Vector3.down, col.bounds.extents.y + 0.2f, groundLayer);

            return hit;
        }
        else
        {
            bool hit;
            // 내가 지금 바닥인지
            hit = Physics.Raycast(boxCol.bounds.center, Vector3.down, col.bounds.extents.y + 0.2f, groundLayer);

            return hit;
        }
    }

    [PunRPC]
    public void JumpAnim()
    {
        animator.SetTrigger("Jump");
        Jump();
    }

    [PunRPC]
    public void MoveAnim(float degree)
    {
        animator.SetFloat("Blend", degree);
    }

    [PunRPC]
    public void OnMoveRPC()
    {
        animator.SetBool("IsCast", false);
    }
}
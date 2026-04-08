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
    private CinemachineThirdPersonFollow thirdPersonFollow;
    [Header("카메라 수직 이동")]
    [SerializeField] private float cameraSpeed = 20f;
    [SerializeField] private float maxShoulderOffsetY = 16f;
    [Header("이동 충돌 보정 거리")]
    [SerializeField] private float wallSkin = 0.01f;
    
    
    [Header("땅 레이어")] [SerializeField]
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
            thirdPersonFollow = playerCam.GetComponent<CinemachineThirdPersonFollow>();
            // 마우스 커서  안보이게
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
        // 카메라 상하
        // 맥 -> 왼 command  / 윈도우 -> 왼 alt
        if((Input.GetKey(KeyCode.LeftAlt) ||  Input.GetKey(KeyCode.LeftCommand))
           && thirdPersonFollow !=null)
        {
            Vector3 shoulder = thirdPersonFollow.ShoulderOffset;
            shoulder.y += mouseY * cameraSpeed;
            shoulder.y = Mathf.Clamp(shoulder.y, 0, maxShoulderOffsetY);
            thirdPersonFollow.ShoulderOffset = shoulder;
        }
        
        // 회전
        Quaternion rot = Quaternion.Euler(0, mouseX * playerState.rotationSpeed, 0);
        rb.MoveRotation(rb.rotation * rot);

        isGrounded = Grounded();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return; // 점프도 내 캐릭터만
        if(GameManager.isCinematic) return;
        if (rb.linearVelocity.y < 0)
        {
            // 중력 가속도 높이기 -> 떨어질 떄 팍 떨어지게
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (4f - 1) * Time.fixedDeltaTime;
        }

        if (canMove)
        {
            // 이동 방향 한번에
            Vector3 inputDir = transform.forward * v + transform.right * h;
            // 방향 길이 1로 제한 후 실제 움직일 거리
            Vector3 moveDelta = Vector3.ClampMagnitude(inputDir, 1f) * (playerState.speed * Time.fixedDeltaTime);

            // 이동량이 있다면 
            if (moveDelta.sqrMagnitude > 0f)
            {
                // 이동
                rb.MovePosition(rb.position + GetResolvedMove(moveDelta));
            }
        }
    }

    private Vector3 GetResolvedMove(Vector3 moveDelta)
    {
        // 벽이 없으면
        // SweepTest -> rb가 방향으로 이동할 때 앞에 뭐가 부딪히는 지
        // QueryTriggerInteraction.Ignore -> 트리거 콜라이더 무시
        if (!rb.SweepTest(moveDelta.normalized, out RaycastHit hit, moveDelta.magnitude + wallSkin,
                QueryTriggerInteraction.Ignore)) 
        {
            return moveDelta; // 벽이 없으면 원래 이동거리.
        }
        
        // 벽이 있으면
        // 지금거리에서 벽까지 거리 (음수는 안나오게)
        float moveDistance = Mathf.Max(hit.distance - wallSkin, 0f);
        // 벽을 뚫지 않고 갈 수 있는 최대거리
        Vector3 resolvedMove = moveDelta.normalized * moveDistance;

        // remainMove -> 벽 때문에 못갈 거리.(남은 이동량)
        Vector3 remainMove = moveDelta - resolvedMove;
        // ProjectOnPlane -> 남은 이동량 중에서 벽으로 파고드는 성분 제고 후 벽표면에 따라 움직이는 성분만 남김
        Vector3 slideMove = Vector3.ProjectOnPlane(remainMove, hit.normal);

        // 미끄러지는 성분이 거의 없다면
        if (slideMove.sqrMagnitude <= 0.0001f)
        {
            // 벽을 뚫지 않고 갈 수 있는 최대거리만큼만 이동
            return resolvedMove;
        }
        
        // 미끄러지는 이동 방향앞에 벽이 있는지
        if (rb.SweepTest(slideMove.normalized, out RaycastHit slideHit, slideMove.magnitude + wallSkin,
                QueryTriggerInteraction.Ignore))
        {
            // 벽이 있다면
            // 벽(앞에 조금 남기고)까지의 최대거리
            float slideDistance = Mathf.Max(slideHit.distance - wallSkin, 0f);
            // 기존 미끄러지는 이동 방향에 최대로 갈 수 있는 거리
            slideMove = slideMove.normalized * slideDistance;
        }
        // 벽 직전까지 이동 최대거리 + 미끄러지는 이동 최대거리 
        return resolvedMove + slideMove;
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
            hit = Physics.Raycast(boxCol.bounds.center, Vector3.down, boxCol.bounds.extents.y + 0.2f, groundLayer);

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

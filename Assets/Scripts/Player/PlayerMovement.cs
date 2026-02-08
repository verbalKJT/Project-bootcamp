using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    [Header("컴포넌트들")] [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private CapsuleCollider col;

    [Header("플레이어 데이터")] [SerializeField] PlayerState playerState;

    [SerializeField] Transform playerCam;

    private float h, v;
    private float mouseX, mouseY; // 마우스로 회전 조정
    private float sensitivity = 30f;
    private float spaceBar;

    private bool isGrounded = true;

    [Header("Terrain 레이어")] [SerializeField]
    private LayerMask groundLayer;

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

        if (Input.GetKeyDown(KeyCode.Space) && Grounded()) // 스페이스바를 누르면
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.y < 0)
        {
            // 중력 가속도 높이기 -> 떨어질 떄 팍 떨어지게
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (4f - 1) * Time.fixedDeltaTime;
        }
    }

    private void Jump()
    {
        float height = col.height; // 콜라이더(캐릭터) 키 
        float jumpVelocity = Mathf.Sqrt(height * -2 * Physics.gravity.y); // v = (2gh)^2 공식사용해서 힘 구하기
        
        rb.AddForce(Vector3.up * jumpVelocity * 1.5f, ForceMode.VelocityChange); // 공식 활용 점프 
                                    // 에셋 Transform Scale이 1.5라
    }

    // 바닥 확인
    private bool Grounded()
    {   
        float raycastDistance = (col.height/2)+0.1f;
        bool hit;
        // 내가 지금 바닥인지
        hit = Physics.Raycast(transform.position, Vector3.down,raycastDistance, groundLayer);

        return hit;
    }
}
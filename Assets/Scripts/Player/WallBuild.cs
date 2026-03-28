using Photon.Pun;
using UnityEngine;

public class WallBuild : PlayerAttack
{
    [Header("벽")] [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject preWallPrefab;

    [Header("벽 설치 설정")] private float _dis = 15f;
    private float _duration = 5f;
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform rayPoint;
    
    private bool isAiming = false;
    private GameObject curPreWall;

    private RockAttack rk;
    
    private Camera pCam;

    private float yRot = 0f;
   
    void Start()
    {
        rk = GetComponent<RockAttack>();
        pCam = Camera.main;
        lineRenderer.positionCount = 2; 
        lineRenderer.enabled = false;
    }
    
    void Update()
    {
        base.Update();
        if(commandE && rk.wallTime  >= rk.wallCool)
        {
            rk.photonView.RPC("SummonWall", RpcTarget.All, true);
            if (!isAiming) // 설치 중이 아니라면 
            {
                StartAiming(); // 조준 -> RockAttack.cs 사용불가
                yRot = 0f;
            }
            else
            {
                // 조준 중이면 E키로 회전 
                yRot += 90f;
                if (yRot >= 360f) yRot = 0f;
            }
        }

        if (isAiming)
        {
            // 프리뷰 위치 갱신
            UdpatePreViewPos();
            
            // 조준중 R키로 취소. 
            if (Input.GetKeyDown(KeyCode.R))
            {
                isAiming = false;
                CancleAming();
            }

            if (input)
            {
                CreateWall(); // 벽 생성
            }
        }
    }

    private void StartAiming()
    {
        isAiming = true; 
        //rk.enabled = false; // 다른 공격 막기

        curPreWall = Instantiate(preWallPrefab); // 프리뷰는 동기화 할 필요가 없음,
        curPreWall.SetActive(false); // 처음엔 안보이게
        lineRenderer.enabled = true; // 보이도록 
    }

    // 설치 
    private void CreateWall()
    {
        // 네트워크 생성
        if(!PhotonNetwork.IsMasterClient) return;

        GameObject wall = PhotonNetwork.Instantiate("Heroes/" + wallPrefab.name, curPreWall.transform.position,
            curPreWall.transform.rotation);
        rk.SetWallTime(0f); // 초기화
        CancleAming(); // 벽 설치 후 조준 취소
    }

    private void CancleAming()
    {
        rk.enabled = true; // 다른 공격 활성화
        isAiming = false; // 조준 취소
        lineRenderer.enabled = false; // 라인 렌더러 끄기
        // Photon 받아와야됨.
        if (curPreWall != null)
        {
            // 네트워크에서 삭제할 필요 없음
            Destroy(curPreWall);
        }
        rk.photonView.RPC("SummonWall", RpcTarget.All, false);
    }

    private void UdpatePreViewPos()
    {
        // 카메라의 전방 방향
        Vector3 camDir = pCam.transform.forward;
        
        // 레이저 시작 위치 -> 플레이어 머리
        Vector3 startPos = rayPoint.position;;
        RaycastHit hit;

        Ray ray = pCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit,50f,floorMask ))
        {
            // 플레이어와 ray 충돌 지점과의 거리 
            float distance = Vector3.Distance(transform.position, hit.point);
            
            // 너무 멀면 설치 불가능 처리
            if (distance <= _dis)
            {
                
                curPreWall.SetActive(true);
                curPreWall.transform.position = hit.point;
                
                // 시각화
                UdpateLineRender(startPos, hit.point);
            }
            else
            {
                // 최대 사거리 지점에 프리뷰를 강제로 위치시키고 싶다면 아래처럼 처리
                Vector3 dir = (hit.point - transform.position).normalized;
                curPreWall.transform.position = transform.position + (dir * _dis);
                curPreWall.SetActive(true); 
                // 시각화
                UdpateLineRender(startPos, hit.point);
            }
            // 벽 회전 적용.
            curPreWall.transform.localRotation = Quaternion.identity * Quaternion.Euler(0, yRot, 0);;
        }
    }

    private void UdpateLineRender(Vector3 startPos, Vector3 endPos)
    {
        if (lineRenderer.enabled && lineRenderer != null)
        {
            // 시작 위치 끝 위치 설정.
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }
}